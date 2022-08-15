﻿using System.Net.Http.Json;
using LukeParkerDev.Blog.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LukeParkerDev.Blog.Services;

public class BlogService
{
    private const string StaticFilePath = "_content/LukeParkerDev.Blog/";
    
    private readonly HttpClient _httpClient;
    
    private IReadOnlyList<BlogIndex>? _cachedIndex;
    private DateTime? _cacheExpiry;
    
    private readonly IDeserializer _yamlDeserializer = new DeserializerBuilder()
        .WithNamingConvention(HyphenatedNamingConvention.Instance)
        .Build();
    
    private string GetCacheBusterQuery() => "?cacheBuster=" + Guid.NewGuid();

    public BlogService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<BlogIndex>> GetAllPostsAsync()
    {
        if (_cacheExpiry is not null && _cachedIndex is not null)
        {
            // We have a cache
            if (_cacheExpiry <= DateTime.Now)
            {
                // Cache has expired
                _cacheExpiry = null;
                _cachedIndex = null;
            }
            else
            {
                // Cache is still good
                return _cachedIndex;
            }
        }
        
        var index = await _httpClient.GetFromJsonAsync<IReadOnlyList<BlogIndex>>(StaticFilePath + "index.json" + GetCacheBusterQuery());

        // Update the cache
        _cachedIndex = index ?? ArraySegment<BlogIndex>.Empty;
        _cacheExpiry = DateTime.Now.AddHours(1);
        
        return _cachedIndex;
    }

    public async Task<LoadedBlog?> GetPostAsync(string? slug)
    {
        var index = await GetAllPostsAsync();
        
        var post = index.FirstOrDefault(p => p.Frontmatter.slug == slug);
        
        if (post is null)
        {
            return null;
        }
        
        var blogRaw = await _httpClient.GetStringAsync(StaticFilePath + "posts/" + post.FileName + GetCacheBusterQuery());
        return ParseBlog(blogRaw);
    }

    public LoadedBlog ParseBlog(string blogRaw)
    {
        var sections = blogRaw.Split("+++");

        // Less than 3 sections than this blog is probably an empty file, or invalid possibly - either way its malformed
        if (sections.Length < 3)
        {
            return new LoadedBlog();
        }
        
        var yamlSection = sections[1];
        var markdownSection = sections[2];
            
        var parsedYaml = _yamlDeserializer.Deserialize<BlogFrontmatter>(yamlSection);

        return new LoadedBlog
        {
            Frontmatter = parsedYaml,
            Markdown = markdownSection
        };
    }
}