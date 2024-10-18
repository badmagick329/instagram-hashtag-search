namespace InstagramHashtags.Console.Client;

using InstagramHashtags.Console.Domain;
using System;


public class ClientWithCache : IClient
{
    IgClient IgClient { get; set; }
    ClientCache ClientCache { get; set; }
    public ClientWithCache(string host, string key)
    {
        IgClient = new IgClient(host, key);
        ClientCache = new ClientCache();
    }

    public async Task<string> SearchTag(Tag tag)
    {
        DateTime now = DateTime.UtcNow;
        if (ClientCache.GetTag(tag) is string cache)
        {
            Console.WriteLine($"Search took: {(DateTime.UtcNow - now).Milliseconds}ms");
            Console.WriteLine("[HIT]");
            return cache;
        }
        Console.WriteLine("[MISS]");
        string response = await IgClient.SearchTag(tag);
        Console.WriteLine($"Search took: {(DateTime.UtcNow - now).Milliseconds}ms");
        ClientCache.SetTag(tag, response);
        return response;
    }
}