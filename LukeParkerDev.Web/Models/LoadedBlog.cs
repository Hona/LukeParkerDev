namespace LukeParkerDev.Web.Models;

public class LoadedBlog
{
    public YamlFrontmatter Frontmatter { get; set; } = new();
    public string Markdown { get; set; } = string.Empty;
}