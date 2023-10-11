namespace LukeParkerDev.Blog.Models;

public class BlogFrontmatter : IEquatable<BlogFrontmatter>
{
    public string date { get; set; } = string.Empty;
    public string title { get; set; } = string.Empty;
    public string hook { get; set; } = string.Empty;
    public string slug { get; set; } = string.Empty;
    public string imageUrl { get; set; } = string.Empty;
    public List<string> tags { get; set; } = new();
    public List<string> series { get; set; } = new();
    
    public bool Equals(BlogFrontmatter? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return date == other.date &&
               title == other.title &&
               hook == other.hook && 
               slug == other.slug && 
               imageUrl == other.imageUrl && 
               tags.SequenceEqual(other.tags) && 
               series.SequenceEqual(other.series);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((BlogFrontmatter)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(date, title, hook, slug, imageUrl, tags, series);
    }

    public static bool operator ==(BlogFrontmatter? left, BlogFrontmatter? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(BlogFrontmatter? left, BlogFrontmatter? right)
    {
        return !Equals(left, right);
    }
}