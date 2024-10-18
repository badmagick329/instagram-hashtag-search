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
        Config config = new AppConfig(FilePath).ReadConfig();
        Tag tag = new(args[0]);
        ClientWithCache client = new(config.ApiHost, config.ApiKey);
        List<ApiResponse> apiResponses = await client.SearchTag(tag);
        ParsedPosts parsedPosts = new(apiResponses);
        // Console.WriteLine(parsedPosts.Posts[0]);
        // Console.WriteLine("------------------------");
        // Console.WriteLine(parsedPosts.Posts[^1]);
        PostsOutput output = new(parsedPosts.Posts);
        output.WritePostsToCsv(tag.Text);
        output.WritePostsAnalyticsToFile(tag.Text, tag.Text);
    }
}
