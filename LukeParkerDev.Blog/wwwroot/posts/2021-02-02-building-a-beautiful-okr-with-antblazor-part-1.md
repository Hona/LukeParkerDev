+++ 
date: "2021-02-02"
title: "Building a Beautiful OKR with AntBlazor - Part 1"
slug: "building-a-beautiful-okr-with-antblazor/1"
tags: ["Blazor", "AntDesign", "AntBlazor", "C#"]
series: ["OKR"]
hook: "Setup the project boilerplate, the get DevOps working with Docker, Docker-Compose, GitHub Actions, Cloudflare, NGINX and your VPS of choice!"
+++

## Boilerplate

### GitHub

1. Create a repository called `OKR`, and initialize it with only a README.md file
2. Open GitHub Desktop, and clone the repository to your machine

### Folder Structure

The physical folder structure will be:
```
OKR
│   README.md
│   Docker Related files
|   .sln    
│   ...
|
└───src
    │   
    └─── ... (Projects e.g. 'OKR.Web')
        │   .csproj
        │   Dockerfile
        │   ...
```

### Setup Solution + Projects

1. Open the folder in the file explorer, and `Shift + Right Click -> Open PowerShell window here`. Type `dotnet new gitignore` to generate a .gitignore file. Feel free to make this file the first git commit
2. Create a new empty solution (`Blank Solution` in VS, `Empty Solution` in Rider) and call it `OKR`
3. Create a solution folder `src/` - this is not a physical folder, its part of the solution
4. Create a solution folder `Solution Items`, and add the existing items, README.md and .gitignore. In the future the docker files will be added here too
5. Add 4 projects to the `src/` solution folder, and the `src/` physical folder path - these are following the DDD principals
    * `OKR.Web` **Blazor Server** - here is where the Blazor frontend code will be, separate from business logic + data access layers
    * `OKR.Application` **Class Library** - here is the business logic layer with references to repository interfaces from the `OKR.Core` layer to abstract data access away from business logic
    * `OKR.Core` **Class Library** - here is the core domain models, and repository interfaces
    * `OKR.Infrastructure` **Class Library** - here are the implementations of repository interfaces using Marten
    
## DevOps

Its always nice to have DevOps from day 0 of your project, so lets add it!

### GitHub CI

To setup the dotnet build + test action, go to the `GitHub Repository -> Actions -> .NET -> Set up this workflow`. The template file works without any changes, because our .sln is in the root directory - so click `Start commit -> Commit directly to the main branch`, add a commit message like `Add dotnet CI`, and finally `Commit new file`

### Docker

First we need a Dockerfile to create an image for the Blazor site. Visual Studio automatically generates Dockerfiles - right click the OKR.Web project, `Add -> Docker Support -> Linux -> Ok`. That's all it takes! When projects get dependencies make sure to run this again (like when you reference projects)

### Docker Compose

Now we need to orchestrate the config for the Blazor site, and include the postgres database all in one.

Create a blank file called `docker-compose.yml` and add it to the `Solution Items/` folder in the .sln.

Fill it out with the following:

```yml
version: "3.8"
services:
  frontend:
    build:
      dockerfile: src/OKR.Web/Dockerfile
      context: .
    image: okr-frontend
    container_name: OkrFrontend
    restart: unless-stopped
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_ConnectionStrings__Marten=User ID = okr;Password=o32342134k4r%Y#%Y345yRasdf;Server=postgres;Port=5432;Database=okr_db;Integrated Security=true;Pooling=true
    volumes:
      - ./config/appsettings.json:/app/appsettings.json
    ports:
    - "9595:80"
  postgres:
    image: postgres
    container_name: OkrPostgres
    environment:
      - POSTGRES_USER=okr
      - POSTGRES_PASSWORD=o32342134k4r%Y#%Y345yRasdf
      - POSTGRES_DB=okr_db
    volumes:
      - ./data:/var/lib/postgresql/data
```

Note the two services, the frontend and database. The secrets for the database are hardcoded - this is fine since the database is not exposed to the internet, but for added security feel free to make this in a config file.

The frontend is exposed to the VPS's port 9595, proxying the request to the containers port 80.

To start the whole system you can run `docker-compose up -d --build` (`-d` detached mode, `--build` builds the Dockerfile)

Ensure on your VPS you have installed Git, Docker and Docker Compose.

### Domain + Reverse Proxy

#### Cloudflare

This site will run on a subdomain `okr.lukeparker.dev`, so I will walk through that. (If you want to point it to a root domain, like `lukeparker.dev` use an `A` record pointing to your VPS IP address instead)

Since I have a record for `lukeparker.dev` to point to my VPS, I just need to add a CNAME record to proxy `okr.lukeparker.dev` to the same IP as `lukeparker.dev`.

Open up the Cloudflare dashboard and goto the site (in my case `lukeparker.dev`), the click the DNS page. Click `Add record`:

* Change Type to: `CNAME`
* Change Name to: `okr`
* Change Content to: `@` (`@` will resolve to the base domain - `lukeparker.dev`)

Then click save.

This may take a minute or two to update in Cloudflare's network, but it is pretty fast.

#### NGINX on VPS

Ok so now we have requests incoming, we need to proxy `okr.lukeparker.dev -> localhost:9595`

I have a global NGINX docker-compose repository you can reference that I use for my VPS and sites: [azetio-nginx](https://github.com/Hona/azetio-nginx)

Before we begin you need to install the Cloudflare Origin Certificate on your server, so follow [this guide](https://support.cloudflare.com/hc/en-us/articles/115000479507-Managing-Cloudflare-Origin-CA-certificates) - make sure to note where and what the certificates are called. If you are running your global NGINX server in Docker, make sure to mount the certificate files. [Read more here](https://blog.cloudflare.com/cloudflare-ca-encryption-origin/)

If you used my azetio-nginx, then add a new file in nginx/sites-enabled/ called `lukeparker.dev.okr.conf` and fill it out with the following (renamed to match your domain):

```nginx
server {
    listen 80;
    server_name okr.lukeparker.dev www.okr.lukeparker.dev;

    return 301 https://okr.lukeparker.dev$request_uri;
}

server {
    listen 443 ssl;
    server_name okr.lukeparker.dev www.okr.lukeparker.dev;

    ssl_certificate /etc/ssl/certs/lukeparker.pem;
    ssl_certificate_key /etc/ssl/private/lukeparker.pem;
    ssl_client_certificate /etc/ssl/certs/origin-pull-ca.pem;
    ssl_verify_client on;

    ssl_session_cache builtin:1000 shared:SSL:10m;
    ssl_protocols TLSv1 TLSv1.1 TLSv1.2;
    ssl_ciphers HIGH:!aNULL:!eNULL:!EXPORT:!CAMELLIA:!DES:!MD5:!PSK:!RC4;
    ssl_prefer_server_ciphers on;

    location / {
        proxy_pass http://localhost:9595;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_read_timeout 3600;
    }
}
```

This configuration will automatically redirect http -> https, and proxy requests to the 9595 port, passing forwarded headers.

Note that if you aren't using my nginx config: add this to the `http { ... }` clause:

```nginx
map $http_upgrade $connection_upgrade {
        default Upgrade;
        '' close;
    }
```

Make sure to read [Host ASP.NET Core on Linux with Nginx](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-5.0) to understand the config.

With all this setup, make sure to restart the azetio-nginx config with `docker-compose up -d --force-recreate`

On the VPS, we need to first clone the git repository, so run `git clone git@github.com:Hona/OKR` (Replace with your username + repository)

Navigate to the repo: `cd OKR`

Make sure to create the config directory and an empty appsettings.json file:

```shell
mkdir config
echo "{ }" > "config/appsettings.json"
```

Then start the OKR project with 
`docker-compose up -d --build`

If all goes well, navigate to the domain you setup, and you should see the default Blazor page, if you do, everything is setup perfectly!

---

In the next post, I will plan and setup the core models, repository interfaces and mock repositories.

[← Part 0](https://lukeparker.dev/blog/building-a-beautiful-okr-with-antblazor/0) | [Part 2 →](https://lukeparker.dev/blog/building-a-beautiful-okr-with-antblazor/2)