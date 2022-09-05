using System.Globalization;
using System.Text.Json;
using System.Threading.Channels;
using LukeParkerDev.BuildJob;
using LukeParkerDev.Blog.Models;
using LukeParkerDev.Blog.Services;
using X.Web.Sitemap;

Console.WriteLine("*** Starting Build Job ***");

Console.WriteLine("> Generating blog index ");

var currentCodeDirectory = SourceCode.GetSourceCodeDirectory();

var postsDirectory = currentCodeDirectory + "/../LukeParkerDev.Blog/wwwroot/posts/";

var blogPaths = Directory.GetFiles(postsDirectory, "*.md", SearchOption.AllDirectories);

var blogService = new BlogService(null!);

var index = new List<BlogIndex>();

foreach (var blogPath in blogPaths)
{
    var blogString = await File.ReadAllTextAsync(blogPath);

    var blog = blogService.ParseBlog(blogString);

    var blogIndex = new BlogIndex
    {
        FileName = blogPath.Replace(postsDirectory, ""),
        Frontmatter = blog.Frontmatter
    };
    
    index.Add(blogIndex);
}

#if DEBUG
    var indexString = JsonSerializer.Serialize(index, new JsonSerializerOptions(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    });
#else
    var indexString = JsonSerializer.Serialize(index, new JsonSerializerOptions(JsonSerializerDefaults.Web));
#endif
await File.WriteAllTextAsync(postsDirectory + "../index.json", indexString);
Console.WriteLine("Done");
Console.WriteLine();
Console.WriteLine("> Generating sitemap ");

var sitemap = new Sitemap();

var baseUri = "https://lukeparker.dev";

Console.WriteLine("Generating blog locations");
foreach (var blog in index)
{
    sitemap.Add(new Url
    {
        Location = baseUri + $"/blog/{blog.Frontmatter.slug}",
        LastMod = blog.Frontmatter.date,
        Priority = 0.9,
        ChangeFrequency = ChangeFrequency.Daily
    });
}

Console.WriteLine("Generating static locations");
sitemap.Add(new Url
{
    Location = baseUri + "/blog",
    LastMod = index
        .Select(x => x.Frontmatter.date)
        .OrderByDescending(x => DateTime.ParseExact(x, "yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo))
        .First(),
    Priority = 1,
    ChangeFrequency = ChangeFrequency.Daily
});

sitemap.Add(new Url
{
    Location = baseUri + "/",
    Priority = 1,
    ChangeFrequency = ChangeFrequency.Weekly
});

var sitemapPath = currentCodeDirectory + "/../LukeParkerDev.Web/wwwroot/sitemap.xml";

await sitemap.SaveAsync(sitemapPath);

Console.WriteLine("Done");


