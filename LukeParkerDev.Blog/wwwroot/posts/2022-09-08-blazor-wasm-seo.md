+++
categories: ["Development"]
date: 2022-09-08
slug: "blazor-wasm-seo"
tags: ["C#", "Blazor", "MudBlazor", "SEO", "WASM", "Googlability"]
title: "Blazor WASM SEO - You have a broken website according to Google!"
hook: "A quick and painless guide to fixing your Blazor WASM SEO."
+++

## Introduction

Blazor WebAssembly is a great framework for building web applications. It's fast, easy to use, and has a great community. However, there is one thing that is often overlooked when building a Blazor WebAssembly application, and that is SEO. In this post, I will go over the problem and how to fix it.

## The Problem

As we know Blazor WASM is a static site, however it requires JavaScript to run on the client and load the .NET Framework using WebAssembly. This means when a crawler like GoogleBot visits the site to index it & load information the default index.html is served.

Basically the only text that is 'human readable' by default in this file is:

```html
<div id="blazor-error-ui">
    An unhandled error has occurred.
    <a href="" class="reload">Reload</a>
    <a class="dismiss">🗙</a>
</div>
```

So what does that mean for your SEO? 

Well, what google thinks your site is, is a broken website.

![Bad SEO - Google Search Result](_content/LukeParkerDev.Blog/uploads/lukeparkerdev-bad-seo.png)

## The Quick Solution

If you are just looking for general accuracy for your site, you can just add `<meta>` tags to the head of your index.html file. This will tell Google what your site is about, and will help with the accuracy of your search results.

An example of this is:

```html
<meta Name="keywords" Content="Keywords" />
<meta Property="og:title" Content="Title" />
<meta Name="twitter:title" Content="Title" />
<meta Name="description" Content="SubTitle" />
<meta Property="og:description" Content="SubTitle" />
<meta Name="twitter:description" Content="SubTitle" />
```

Much better than the error message you get by default!

However, this is not a great solution.

1. It still won't load any of the page which helps with keywords and ranking metrics.
2. What if you have a blog? 
3. What if you have subpages that should have descriptions?

## The Better Solution - Prerendering for Crawlers

The better solution is to prerender the pages for crawlers. This means that when a crawler visits the site, it will get a fully loaded and cached version of your site. This won't impact your users at all, and allows you to use a free hosting service like GitHub Pages, or Azure Static Web Apps.

**The Benefits**

* Custom `<meta>` tags and SEO data for each page. Completely customizable in your Razor components!
* No impact on your users, as the prerendering is only for crawlers.

### Prerendering with Blazor

A prerequisite to this way of prerending is that you use Cloudflare for your DNS.

In my case lukeparker.dev is hosted on GitHub Pages & CloudFlare pages, with CloudFlare as the DNS provider.

#### Prerender.io - A SPA Prerendering Service for Crawlers

> I am not sponsored or affiliated with prerender.io in any way. 
> I use the free service and it works well.

[prerender.io](https://prerender.io/) is a service that allows you to prerender your site for crawlers. It's free for up to 1000 prerenders per month, which is more than enough for most sites (roughly 250 unique pages).

It utilizes an industry standard for this problem which is Dynamic Rendering. Which, like I outlined is basically prerendering the site for crawlers, while serving the normal site for users.

#### Setting it Up

Thankfully instead of me maintaining a third party manual, they have a CloudFlare integration guide [here](https://docs.prerender.io/docs/24-cloudflare).

Basically you copy a little JavaScript code into a new Worker on CloudFlare & setup your API key & domain.

At that point you're good to go & can start using prerender.io!

---

## Blazor SEO - Page-based Dynamic Metadata 

> Note: My implementation is heavily based on [MudBlazor's](https://github.com/MudBlazor/MudBlazor/blob/dev/src/MudBlazor.Docs/Components/DocsPageHeader.razor#L10-L22) which is what prompted me to go down the SEO route with Blazor WASM.

Firstly we need to create a component that will be used to set the metadata for each page. This is a simple component that will take in a title, description, and keywords.

Create a file called `SeoHeader.razor`.

Then lets fill it with the following:

```html
<PageTitle>@(GetTitle())</PageTitle>
<HeadContent>
    <meta Name="keywords" Content="@GetKeywords()" />
    <meta Property="og:title" Content="@GetTitle()" />
    <meta Name="twitter:title" Content="@GetTitle()" />
    @if (!string.IsNullOrEmpty(Overview))
    {
        <meta Name="description" Content="@GetSubTitle()" />
        <meta Property="og:description" Content="@GetSubTitle()" />
        <meta Name="twitter:description" Content="@GetSubTitle()" />
    }
</HeadContent>
```

Note the usage of `<PageTitle>` to set the tab title & the `<HeadContent>` to set part of the `<head>` of the page using the new HeadOutlet.

Now we just need to fill out the code that takes these variables and returns the nice strings.

Add this code at the bottom of the component:

```cs
@code {
    [Parameter]
    public string? Title { get; set; }
    
    [Parameter]
    public string? Overview { get; set; }
    
    [Parameter]
    public IEnumerable<string> Keywords { get; set; } = new List<string>();
        
    string GetTitle() => Title is null ? "LukeParkerDev" : $"{Title} | [YOUR BRAND NAME/SUFFIX]";
        
    string GetSubTitle()
    {
        if (string.IsNullOrEmpty(Overview))
            return "";
        return Overview.TrimEnd('.') + ".";
    }
    
    string GetKeywords()
    {
        var keywords = new List<string>();
        
        keywords.AddRange(Keywords);
        
        keywords.Add("[YOUR NAME or BRAND]");
    
        return string.Join(", ", keywords);
    }
}
```

At this point the component is ready to be consumed in every Razor Page.

### Using the SEO Header Component

Starting simple, lets add it to the Index page.

```html
<SeoHeader
    Overview="My personal website acting as a personal portfolio as well as a blog to share my knowledge." 
    Keywords="@(new []{"blog", "portfolio"})"/>
```

This one just sets a description & keywords. Remember it always adds the one keyword of your brand name.

Now lets add it to something more dynamic, like a blog post, dynamically loaded

```html
@if (_loading)
{
    <MudProgressLinear Color="Color.Secondary" Indeterminate/>
}
else if (_blog is not null)
{
    <SeoHeader 
            Title="@_blog.Frontmatter.title" 
            Overview="@_blog.Frontmatter.hook" 
            Keywords="_blog.Frontmatter.categories
                .Concat(_blog.Frontmatter.series)
                .Concat(_blog.Frontmatter.tags)" />
    ...
}
```

Thats all! Now your site will slowly get crawled by Google & others, in a few days you'll see the updated version of your site!

### The Result

This exact site uses that approach, and here is an example of the result in Discord:

![Good SEO - Discord Embed](_content/LukeParkerDev.Blog/uploads/seo-result-discord.png)


![Good SEO - Google](_content/LukeParkerDev.Blog/uploads/seo-result-google.png)

Even better is MudBlazor's, which has a lot more pages & content:

![Good SEO - MudBlazor on Google](_content/LukeParkerDev.Blog/uploads/seo-result-mudblazor-google.png)

### Conclusion

I hope this post has helped you understand how to do SEO with Blazor WASM. If you have any questions, feel free to ask in the comments!

### Next Steps

You might want to generate a SiteMap dynamically, or have one statically, which helps Google & other crawlers find your pages much more easily! You can see how I do it [here](https://github.com/Hona/LukeParkerDev/blob/main/LukeParkerDev.BuildJob/Program.cs) on my blog's Build Job.
