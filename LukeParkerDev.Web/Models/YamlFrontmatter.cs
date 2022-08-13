namespace LukeParkerDev.Web.Models;

public class YamlFrontmatter
{
    public string date { get; set; }
    public string title { get; set; }
    public string hook { get; set; }
    public string slug { get; set; }
    public string imageUrl { get; set; }
    public List<string> tags { get; set; }
    public List<string> categories { get; set; }
    public List<string> series { get; set; }
}