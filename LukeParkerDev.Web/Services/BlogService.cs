using LukeParkerDev.Web.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LukeParkerDev.Web.Services;

public class BlogService
{
    private readonly HttpClient _httpClient;
    
    private IReadOnlyList<LoadedBlog>? _cachedBlogs;
    private DateTime? _cacheExpiry;

    public static readonly string[] BlogFileNames = 
    {
        "/blog/2021-01-07-why-i-switched-to-rider-from-vs-for-csharp.md",
        "/blog/2021-02-01-building-a-beautiful-okr-with-antblazor-part-0.md",
        "/blog/2021-02-02-building-a-beautiful-okr-with-antblazor-part-1.md",
        "/blog/2021-02-03-building-a-beautiful-okr-with-antblazor-part-2.md",
        "/blog/2021-02-04-building-a-beautiful-okr-with-antblazor-part-3.md"
    };

    public BlogService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<LoadedBlog>> GetBlogPostsAsync()
    {
        if (_cacheExpiry is not null && _cachedBlogs is not null)
        {
            // We have a cache
            if (_cacheExpiry <= DateTime.Now)
            {
                // Cache has expired
                _cacheExpiry = null;
                _cachedBlogs = null;
            }
            else
            {
                // Cache is still good
                return _cachedBlogs;
            }
        }
        
        var yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(HyphenatedNamingConvention.Instance)
            .Build();
        
        var cacheBuster = "?cacheBuster=" + Guid.NewGuid();
        
        var output = new List<LoadedBlog>();
        
        foreach (var blogFile in BlogFileNames)
        {
            var blogText = await _httpClient.GetStringAsync(blogFile + cacheBuster);
            var sections = blogText.Split("+++");

            var yamlSection = sections[1];
            var markdownSection = sections[2];
            
            var parsedYaml = yamlDeserializer.Deserialize<YamlFrontmatter>(yamlSection);

            output.Add(new LoadedBlog
            {
                Frontmatter = parsedYaml,
                Markdown = markdownSection
            });
        }

        // Update the cache
        _cachedBlogs = output.AsReadOnly();
        _cacheExpiry = DateTime.Now.AddHours(1);
        
        return _cachedBlogs;
    }
}