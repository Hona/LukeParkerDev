+++
date: 2022-09-13
slug: "rapid-development-with-marten-db"
tags: ["C#", "Database", "Marten", "DDD", "Document Store", "NoSQL", "Minimal API", "MediatR"]
title: "Rapid Backend Development with Marten DB"
hook: "Building a personal project can be exciting, but it can also be a lot of work. In this post, I will go over how to use Marten DB to quickly build a backend for your project."
+++

## Introduction

Building a personal project can be exciting, but it can also be a lot of work. In this post, I will go over how to use Marten DB to quickly build a backend for your project.

### The Problem

When building a project, you will often need to build a backend for it. This can be a lot of work, and it's easy to get distracted by the infrastructure and forget about the project. In this post, I will go over how to use Marten DB to quickly build a backend for your project.

### The Solution

Marten DB is a fantastic document store that uses PostgreSQL as the database engine. It is easy to use, and allows you to quickly build your backend.

## Marten's Background

Before Marten came around, the de facto Document DB to use was Raven DB. Raven DB was a fantastic Document DB, but it was not free.

The developers behind Marten were working on a 'very large web application' that was suffering with performance and stability issues - that was using RavenDB as the document store. They decided to build their own document store, and Marten was born.

But why the name?

Straight from the Marten DB website:

> The project name Marten came from a quick Google search one day for "what are the natural predators of ravens?" -- which led to us to use the marten as our project codename and avatar.

A bit of a superhero origin story, but it's a great name!

### Setting up Marten DB

First things first lets create a new ASP.NET Core Web API, and add MediatR to it. While we are at it, let's also add Marten.

```bash
dotnet new webapi -o MartenTest
cd MartenTest
dotnet add package Marten
dotnet add package MediatR
```

Now we need to add a connection string to our appsettings.json file. I am using a local PostgreSQL database, but you can use any version Marten supports, which as of right now is: `PostgreSQL 9.6 or above database with PLV8`

`appsettings.Development.json`
```json
{
  "ConnectionStrings": {
    "MartenTest": "Host=localhost;Database=MartenTest;Username=postgres;Password=postgres"
  }
}
```

Next lets setup the pipeline in `Program.cs`

```cs
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("Marten"));

    options.AutoCreateSchemaObjects = builder.Environment.IsDevelopment()
        ? AutoCreate.All
        : AutoCreate.CreateOrUpdate;
});

// For the sake of this article we'll just put everything into one project
builder.Services.AddMediatR(typeof(Program).Assembly);
```

As a homage to the Blazor template using WeatherForecasts as the model of choice, lets build a few commands and queries to perform CRUD operations on.

`MartenTest/Domain/WeatherForecast.cs`
```cs
public record WeatherForecast(DateTime Date, int TemperatureC, string Summary);
```

Now, if you've used Document Stores to any degree before - we need to have a field that acts as the identity or primary key. Marten DB by default looks for the `Id` property, so we need to manually configure it to use `Date` instead.

`MartenTest/Program.cs`
```cs
builder.Services.AddMarten(options =>
{
    ...
    
    options.Schema.For<WeatherForecast>()
        .Identity(x => x.Date);
});
```

#### CREATE

`MartenTest/Application/CreateWeatherForecastCommand.cs`
```cs
public record CreateWeatherForecastCommand(WeatherForecast WeatherForecast) : IRequest<WeatherForecast>;

public class CreateWeatherForecastCommandHandler : IRequestHandler<CreateWeatherForecastCommand, WeatherForecast>
{
    private readonly IDocumentSession _documentSession;

    public CreateWeatherForecastCommandHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<WeatherForecast> Handle(CreateWeatherForecastCommand request, CancellationToken cancellationToken)
    {
        await _documentSession.Insert(request.WeatherForecast);
        await _documentSession.SaveChangesAsync(cancellationToken);

        return request.WeatherForecast;
    }
}
```

#### UPDATE

`MartenTest/Application/UpdateWeatherForecastCommand.cs`
```cs
public record UpdateWeatherForecastCommand(WeatherForecast WeatherForecast) : IRequest<WeatherForecast>;

public class UpdateWeatherForecastCommandHandler : IRequestHandler<UpdateWeatherForecastCommand, WeatherForecast>
{
    private readonly IDocumentSession _documentSession;

    public UpdateWeatherForecastCommandHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<WeatherForecast> Handle(UpdateWeatherForecastCommand request, CancellationToken cancellationToken)
    {
        _documentSession.Store(request.WeatherForecast);
        await _documentSession.SaveChangesAsync(cancellationToken);

        return request.WeatherForecast;
    }
}
```

> An important thing to note is the nomenclature. You would use `Insert` to create a new document, and `Store` to update or create an existing document.

#### DELETE

`MartenTest/Application/DeleteWeatherForecastCommand.cs`
```cs
public record DeleteWeatherForecastCommand(DateTime Date) : IRequest;

public class DeleteWeatherForecastCommandHandler : IRequestHandler<DeleteWeatherForecastCommand>
{
    private readonly IDocumentSession _documentSession;

    public DeleteWeatherForecastCommandHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<Unit> Handle(DeleteWeatherForecastCommand request, CancellationToken cancellationToken)
    {
        _documentSession.Delete<WeatherForecast>(new WeatherForecast(request.Date, 0, ""));
        await _documentSession.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
```

#### READ

`MartenTest/Application/GetWeatherForecastQuery.cs`
```cs
public record GetWeatherForecastQuery(DateTime Date) : IRequest<WeatherForecast>;

public class GetWeatherForecastQueryHandler : IRequestHandler<GetWeatherForecastQuery, WeatherForecast>
{
    private readonly IDocumentSession _documentSession;

    public GetWeatherForecastQueryHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public Task<WeatherForecast> Handle(GetWeatherForecastQuery request, CancellationToken cancellationToken)
    {
        return _documentSession.Query<WeatherForecast>()
            .FirstOrDefaultAsync(x => x.Date == request.Date, cancellationToken);
    }
}
```

Just like that we have a fully functional infrastructure. As a bonus - lets use minimal APIs to expose our CRUD operations.

> Minimal APIs are a new feature in .NET 6 that allows you to build web APIs without having to use controllers. You can read more about them [here](https://devblogs.microsoft.com/aspnet/asp-net-core-updates-in-net-6-preview-4/#minimal-apis).

> MediatR is discussing adding support for minimal APIs [here](https://github.com/jbogard/MediatR/issues/653).

`MartenTest/WebApi/Endpoints.cs`
```cs
public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/weatherforecast/{date}", (DateTime date, IMediator mediator, CancellationToken ct) =>
            mediator.Send(new GetWeatherForecastQuery(date), ct)
        );
        app.MapPost("/weatherforecast", ([FromBody] model, IMediator mediator, CancellationToken ct) =>
            mediator.Send(new CreateWeatherForecastCommand(model), ct)
        );
        app.MapPut("/weatherforecast", ([FromBody] model, IMediator mediator, CancellationToken ct) =>
            mediator.Send(new UpdateWeatherForecastCommand(model), ct)
        );
        app.MapDelete("/weatherforecast/{date}", (DateTime date, IMediator mediator, CancellationToken ct) =>
            mediator.Send(new DeleteWeatherForecastCommand(date), ct)
        );
    }
}
```

`MartenTest/Program.cs`
```cs
var app = builder.Build();

...

app.MapEndpoints();

app.Run();
```

And that's it! We have a fully functional CRUD API that uses Marten DB as the data store. 

## Conclusion

Marten DB is a great tool for building Document Stores in .NET. It's easy to use, and has a lot of great features. I hope this article has helped you get started with Marten DB. If you have any questions or comments, feel free to reach out to me on Twitter [@LukeParkerDev](https://twitter.com/LukeParkerDev).

Please let me know what you think of Marten DB, or this article. I'd love to hear your feedback in the comments below!