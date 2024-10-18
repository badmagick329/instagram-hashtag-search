namespace InstagramHashtags.Console;
using System;
using InstagramHashtags.Console.Client;
using InstagramHashtags.Console.Domain;

class Program
{
    private const string FilePath = "config.yaml";
    static async Task Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Please provide a tag.");
            Environment.Exit(1);
        }
        Console.WriteLine("Reading config.yaml...");
        Config config = new AppConfig(FilePath).ReadConfig();
        Console.WriteLine("Reading tag...");
        Tag tag = new(args[0]);
        Console.WriteLine($"Found tag: {tag.Text}");
        Console.WriteLine("Searching for tag...");
        ClientWithCache client = new(config.ApiHost, config.ApiKey);
        Console.WriteLine("Fetching posts...");
        List<ApiResponse> apiResponses = await client.SearchTag(tag);
        Console.WriteLine("Parsing posts...");
        ParsedPosts parsedPosts = new(apiResponses);
        Console.WriteLine("Creating output...");
        PostsOutput output = new(parsedPosts.Posts);
        output.WritePostsToCsv(tag.Text);
        output.WritePostsAnalyticsToFile(tag.Text, tag.Text);
        Console.WriteLine("Done.");
    }
}
