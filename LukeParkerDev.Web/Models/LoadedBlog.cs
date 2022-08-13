namespace LukeParkerDev.Web.Models;

public class LoadedBlog
{
    public YamlFrontmatter Frontmatter { get; set; }
    public string Markdown { get; set; }
}