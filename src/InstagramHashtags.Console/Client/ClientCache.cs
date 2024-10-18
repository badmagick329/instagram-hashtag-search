namespace InstagramHashtags.Console.Client;

using InstagramHashtags.Console.Domain;

public class ClientCache
{
    private const string CacheDirPath = "cache";
    private DirectoryInfo TagCacheDir { get; set; }
    public ClientCache()
    {
        try
        {
            TagCacheDir = Directory.CreateDirectory(Path.Combine(CacheDirPath, "tags")) ?? throw new Exception("Failed to create tags cache directory");
        }
        catch (Exception e)
        {
            throw new Exception("Failed to create cache directory", e);
        }
    }

    public void SetTag(Tag tag, string response)
    {
        string saveFilename = Path.Combine(TagCacheDir.FullName, $"{DatetimeString()} {tag.Text}.json");
        var tagFile = new FileInfo(saveFilename);
        using var writer = tagFile.CreateText();
        writer.Write(response);
    }

    public string? GetTag(Tag tag)
    {
        FileInfo? tagFile = TagToSavedFilename(tag);
        if (tagFile is null || !tagFile.Exists)
        {
            return null;
        }
        using var reader = tagFile.OpenText();
        return reader.ReadToEnd();
    }

    private FileInfo? TagToSavedFilename(Tag tag)
    {
        string[] files = [
            .. Directory.GetFiles(TagCacheDir.FullName)
            .Where(f => f.Contains(' ') && f.EndsWith(".json"))
            .OrderByDescending(f => f)
        ];
        foreach (string file in files)
        {
            string tagPart = Path.GetFileNameWithoutExtension(file).Split(' ')[1];
            if (tagPart == tag.Text)
            {
                return new FileInfo(file);
            }
        }
        return null;
    }
    private static string DatetimeString() => DateTime.UtcNow.ToString("yyyy-MM-dd__HH_mm_ss");
}