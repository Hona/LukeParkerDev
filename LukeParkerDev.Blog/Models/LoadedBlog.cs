﻿namespace LukeParkerDev.Blog.Models;

public class LoadedBlog
{
    public BlogFrontmatter Frontmatter { get; set; } = new();
    public string Markdown { get; set; } = string.Empty;
}