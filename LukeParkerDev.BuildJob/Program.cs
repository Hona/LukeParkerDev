using System.Globalization;
using System.Text;
using System.Text.Json;
using LukeParkerDev.BuildJob;
using LukeParkerDev.Blog.Models;
using LukeParkerDev.Blog.Services;
using SimpleSiteMap;
using WilderMinds.RssSyndication;

Console.WriteLine("*** Starting Build Job ***");

Console.WriteLine("> Generating blog index ");

var currentCodeDirectory = SourceCode.GetSourceCodeDirectory();

var postsDirectory = currentCodeDirectory + "/../LukeParkerDev.Blog/wwwroot/posts/";

var blogPaths = Directory.GetFiles(postsDirectory, "*.md", SearchOption.AllDirectories);


var index = new List<BlogIndex>();

foreach (var blogPath in blogPaths)
{
    var blogString = await File.ReadAllTextAsync(blogPath);

    var blog = BlogService.ParseBlog(blogString);

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

Console.WriteLine("> Generating RSS feed ");

var feed = new Feed
{
    Title = "LukeParkerDev",
    Description = "My personal website acting as a personal portfolio as well as a blog to share my knowledge.",
    Link = new Uri("https://lukeparker.dev/blog.rss"),
    Copyright = "(c) 2022",
    Language = "en-au"
};

feed.Items = index.Select(x => new Item
{
    Title = x.Frontmatter.title,
    Body = x.Frontmatter.hook,
    Link = new Uri(baseUri, $"/blog/{x.Frontmatter.slug}"),
    Permalink = new Uri(baseUri, $"/blog/{x.Frontmatter.slug}").ToString(),
    PublishDate = DateTime.ParseExact(x.Frontmatter.date, "yyyy-MM-dd", DateTimeFormatInfo.CurrentInfo),
    Author = new Author { Name = "Luke Parker", Email = "lukeparkerdev@outlook.com" },
}).ToList();


var rssPath = currentCodeDirectory + "/../LukeParkerDev.Web/wwwroot/blog.rss";

Console.WriteLine("Saving");

var rssSerialized = feed.Serialize(new SerializeOption
{
    Encoding = Encoding.UTF8
});

File.Delete(rssPath);
await using var rssTextWriter = File.CreateText(rssPath);
await rssTextWriter.WriteAsync(rssSerialized);

Console.WriteLine("Done");


