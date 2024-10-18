namespace InstagramHashtags.Console.Domain;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Text;
using System;

public class PostsOutput
{
    private const string OutputDirPath = "output";
    private DirectoryInfo OutputDir { get; set; }
    public List<ParsedPost> Posts { get; set; } = [];
    public PostsOutput(List<ParsedPost> posts)
    {
        Posts = [.. posts.OrderByDescending(p => p.CommentCount)];
        try
        {
            OutputDir = Directory.CreateDirectory(OutputDirPath);
        }
        catch (Exception e)
        {
            throw new Exception("Failed to create output directory", e);
        }
    }
    public void WritePostsToCsv(string filename)
    {
        string outputFile = Path.Combine(OutputDir.FullName, $"{filename}.csv");
        using var writer = new StreamWriter(outputFile, false, new UTF8Encoding(true));
        CsvWriter csvWriter = new(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        });
        using var csv = csvWriter;
        csv.Context.RegisterClassMap<ParsedPostMap>();
        csv.WriteRecords(Posts);
    }

    public void WritePostsAnalyticsToFile(string filename, string searchedHashTag)
    {
        string outputFile = Path.Combine(OutputDir.FullName, $"{filename}.txt");
        var (averageLikeCount, medianLikeCount) = GetLikeCountMetrics();
        var (averageCommentCount, medianCommentCount) = GetCommentCountMetric();
        Dictionary<string, int> otherHashtags = GetTopHashtags(searchedHashTag);

        using var writer = new StreamWriter(outputFile, false, new UTF8Encoding(true));
        writer.WriteLine($"Metrics for {searchedHashTag}");
        writer.WriteLine($"Average like count: {averageLikeCount}");
        writer.WriteLine($"Median like count: {medianLikeCount}");
        writer.WriteLine($"Average comment count: {averageCommentCount}");
        writer.WriteLine($"Median comment count: {medianCommentCount}");
        writer.WriteLine("Top hashtags:");
        foreach (KeyValuePair<string, int> hashtag in otherHashtags)
        {
            writer.WriteLine($"{hashtag.Key}: {hashtag.Value}");
        }

    }

    private (int, int) GetLikeCountMetrics()
    {
        int averageLikeCount = (int)Posts.Average(p => p.LikeCount);
        int medianLikeCount = 0;
        if (Posts.Count % 2 == 0)
        {
            medianLikeCount = (Posts[Posts.Count / 2].LikeCount + Posts[Posts.Count / 2 - 1].LikeCount) / 2;
        }
        else
        {
            medianLikeCount = Posts[Posts.Count / 2].LikeCount;
        }
        return (averageLikeCount, medianLikeCount);
    }

    private (int, int) GetCommentCountMetric()
    {
        int averageCommentCount = (int)Posts.Average(p => p.CommentCount);
        int medianCommentCount = 0;
        if (Posts.Count % 2 == 0)
        {
            medianCommentCount = (Posts[Posts.Count / 2].CommentCount + Posts[Posts.Count / 2 - 1].CommentCount) / 2;
        }
        else
        {
            medianCommentCount = Posts[Posts.Count / 2].CommentCount;
        }
        return (averageCommentCount, medianCommentCount);
    }

    private Dictionary<string, int> GetTopHashtags(string searchedHashTag)
    {
        if (!searchedHashTag.StartsWith("#"))
        {
            searchedHashTag = $"#{searchedHashTag}";
        }
        Dictionary<string, int> otherHashtags = [];
        foreach (ParsedPost post in Posts)
        {
            foreach (string hashtag in post.Hashtags)
            {
                if (hashtag == searchedHashTag)
                {
                    continue;
                }

                if (!otherHashtags.ContainsKey(hashtag))
                {
                    otherHashtags[hashtag] = 0;
                }

                otherHashtags[hashtag] += 1;
            }
        }
        otherHashtags = otherHashtags.OrderByDescending(h => h.Value).ToDictionary(h => h.Key, h => h.Value);
        return otherHashtags;
    }
}

public class ParsedPostMap : ClassMap<ParsedPost>
{
    public ParsedPostMap()
    {
        Map(m => m.AccessibilityCaption);
        // Map(m => m.Code);
        Map(m => m.Code).Name("Url").Convert(row => $"https://instagram.com/p/{row.Value.Code}");
        Map(m => m.CaptionIsEdited);
        Map(m => m.CommentCount);
        Map(m => m.CommercialityStatus);
        // Map(m => m.HasHighRiskGenAiInformTreatment);
        Map(m => m.HasSharedToFb);
        // Map(m => m.IsPaidPartnership);
        Map(m => m.IsVideo);
        Map(m => m.LikeCount);
        Map(m => m.TopLikers).Convert(row => string.Join(";", row.Value.TopLikers ?? []));
        Map(m => m.Hashtags).Convert(row => string.Join(";", row.Value.Hashtags ?? []));
        Map(m => m.CreatedAtUtc).TypeConverterOption.Format("yyyy/MM/dd HH:mm:ss");
        Map(m => m.CreatedAtIslamabadTime).TypeConverterOption.Format("yyyy/MM/dd HH:mm:ss");
        // Map(m => m.CaptionText);
        Map(m => m.LocationAddress);
        Map(m => m.LocationCity);
        Map(m => m.LocationName);
        Map(m => m.LocationExternalSource);
        Map(m => m.LocationLat);
        Map(m => m.LocationLng);
    }
}