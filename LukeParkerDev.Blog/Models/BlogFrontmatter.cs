namespace LukeParkerDev.Blog.Models;

public class BlogFrontmatter
{
    public string date { get; set; } = string.Empty;
    public string title { get; set; } = string.Empty;
    public string hook { get; set; } = string.Empty;
    public string slug { get; set; } = string.Empty;
    public string imageUrl { get; set; } = string.Empty;
    public List<string> tags { get; set; } = new();
    public List<string> categories { get; set; } = new();
    public List<string> series { get; set; } = new();
}