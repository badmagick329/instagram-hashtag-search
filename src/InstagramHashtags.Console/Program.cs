namespace InstagramHashtags.Console;
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        await client.SearchTag(tag);
    }

    static void Sample()
    {
        string jsonResponse = File.ReadAllText("response.json");
        JObject json = JObject.Parse(jsonResponse);

        if (json["data"]?["items"] is JArray items && items.Count > 0)
        {
            var firstItem = items[0];

            if (firstItem["caption"]?["hashtags"] is JArray hashtags)
            {
                Console.WriteLine("Hashtags:");
                foreach (var hashtag in hashtags)
                {
                    Console.WriteLine(hashtag.ToString());
                }
            }
            else
            {
                Console.WriteLine("Hashtags not found.");
            }
        }
        else
        {
            Console.WriteLine("Items not found.");
        }

    }
}
