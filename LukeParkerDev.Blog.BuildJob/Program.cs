using System.Text.Json;
using LukeParkerDev.Blog.BuildJob;
using LukeParkerDev.Blog.Models;
using LukeParkerDev.Blog.Services;

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