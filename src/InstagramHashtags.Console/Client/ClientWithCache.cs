namespace InstagramHashtags.Console.Client;

using InstagramHashtags.Console.Domain;
using System;
using Newtonsoft.Json;


public class ClientWithCache : IClient
{
    IgClient IgClient { get; set; }
    ClientCache ClientCache { get; set; }
    public ClientWithCache(string host, string key)
    {
        IgClient = new IgClient(host, key);
        ClientCache = new ClientCache();
    }

    public async Task<List<ApiResponse>> SearchTag(Tag tag)
    {
        DateTime now = DateTime.UtcNow;
        if (ClientCache.GetTag(tag) is string cachedData)
        {
            List<ApiResponse>? savedResponses = JsonConvert.DeserializeObject<List<ApiResponse>>(cachedData);
            if (savedResponses is not null)
            {
                Console.WriteLine("Using Saved Data...");
                Console.WriteLine($"Search took: {(DateTime.UtcNow - now).Milliseconds}ms");
                return savedResponses;
            }
        }
        Console.WriteLine("No saved data found. Fetching...");
        List<ApiResponse> apiResponses = await IgClient.SearchTag(tag);
        string secondsFormatted = $"{(DateTime.UtcNow - now).Seconds}.{(DateTime.UtcNow - now).Milliseconds}s";
        Console.WriteLine($"Search took: {secondsFormatted}");
        string apiResponseString = JsonConvert.SerializeObject(apiResponses);
        ClientCache.SetTag(tag, apiResponseString);
        return apiResponses;
    }
}