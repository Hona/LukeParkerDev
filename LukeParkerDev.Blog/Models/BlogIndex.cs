namespace LukeParkerDev.Blog.Models;

public class BlogIndex
{
    public string FileName { get; set; } = string.Empty;
    public BlogFrontmatter Frontmatter { get; set; } = new();
}