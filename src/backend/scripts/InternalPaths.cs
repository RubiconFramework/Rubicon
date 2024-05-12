using System.Collections.Generic;

namespace Rubicon.backend.scripts;

/// <summary>
/// stuff done inside the godot filesystem
/// </summary>
public static class InternalPaths
{
    public static IEnumerable<string> FilesInDirectory(string path)
    {
        List<string> files = new();
        using var directory = DirAccess.Open(path);
        if (directory != null)
        {
            try
            {
                directory.ListDirBegin();
                while (true)
                {
                    string file = directory.GetNext();
                    if (file == "") break;
                    if (!file.StartsWith(".")) files.Add(file);
                }
            }
            finally
            {
                directory.ListDirEnd();
            }
        }
        return files;
    }
}
