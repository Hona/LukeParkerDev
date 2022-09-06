using System.Globalization;
using System.Text.Json;
using LukeParkerDev.BuildJob;
using LukeParkerDev.Blog.Models;
using LukeParkerDev.Blog.Services;
using SimpleSiteMap;

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

var sitemap = new List<SitemapNode>();

var baseUri = new Uri("https://lukeparker.dev");

Console.WriteLine("Generating blog locations");

sitemap.AddRange(index.Select(blog 
    => new SitemapNode(new Uri(baseUri, $"/blog/{blog.Frontmatter.slug}"),
        DateTime.ParseExact(blog.Frontmatter.date, "yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo))));

Console.WriteLine("Generating static locations");
sitemap.Add(new SitemapNode(new Uri(baseUri, "/blog"), DateTime.Now));

sitemap.Add(new SitemapNode(baseUri, DateTime.Now));

var sitemapPath = currentCodeDirectory + "/../LukeParkerDev.Web/wwwroot/sitemap.xml";

var sitemapService = new SitemapService();
var urlSetSerialized = sitemapService.ConvertToXmlUrlset(sitemap);

File.Delete(sitemapPath);
await using var textWriter = File.CreateText(sitemapPath);
await textWriter.WriteAsync(urlSetSerialized);

Console.WriteLine("Saved to " + File.ResolveLinkTarget(sitemapPath, true));

Console.WriteLine("Done");


