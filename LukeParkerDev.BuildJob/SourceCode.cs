using System.Runtime.CompilerServices;

namespace LukeParkerDev.BuildJob;

public class SourceCode
{
    private static string? GetThisFilePath([CallerFilePath] string? path = null)
    {
        return path;
    }

    public static string? GetSourceCodeDirectory()
    {
        var path = GetThisFilePath();
        return Path.GetDirectoryName(path);
    }
}