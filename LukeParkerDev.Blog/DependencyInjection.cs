using LukeParkerDev.Blog.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LukeParkerDev.Blog;

public static class DependencyInjection
{
    public static IServiceCollection AddBlog(this IServiceCollection services)
    {
        services.AddScoped<BlogService>();
        return services;
    }
}