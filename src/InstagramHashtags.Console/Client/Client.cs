namespace InstagramHashtags.Console.Client;

using System.Net.Http.Headers;
using System;
using InstagramHashtags.Console.Domain;

public class IgClient(string host, string key) : IClient
{
    private HttpClient HttpClient { get; set; } = new HttpClient();
    private string ApiHost { get; set; } = host;
    private string ApiKey { get; set; } = key;

    public async Task<string> SearchTag(Tag tag)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            // RequestUri = new Uri($"https://instagram-scraper-api2.p.rapidapi.com/v1/hashtag?hashtag={tag.Text}&pagination_token=Ek1JAAgCPh9eJwQIF0ZHUj4lAjIsNxwLBToGblIIU0pbAisFBzEVVBEkCloWVAwpXwtTPX00HBMHJiYeFh0zPUEjSQIBJhA0A28EPAEhEyc8GV4IVDdUR18FVApcBUocVzNrBEM0XFJbD0dFbDRbH0U2RTdEA0MrSVVVPlVafSdICQUuDAQOUgBIAgEHHQQpfRNDOkpbHkgVHisLCChRF1AaSjIXOz5HMSIRWw5KOG87WSRHOF4IUQ1YKVAyRWdeI2whWTdVK0AQRSxHJkskQiBVMW09OABbLB03ACcxJD4oEiUKPkY9XiRHCRc8BkI8PxQ_EDJKL0Q5VQcHPAExAxYAFwo-FwJKDEA8czsCPAE8ChwQIhs1ETAaZUgWKjcNGEY6ACdRA0cI"),
            RequestUri = new Uri($"https://instagram-scraper-api2.p.rapidapi.com/v1/hashtag?hashtag={tag.Text}"),
            Headers =
            {
                { "x-rapidapi-key", ApiKey },
                { "x-rapidapi-host", ApiHost },
            },
        };
        using var response = await HttpClient.SendAsync(request);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error Message :{e.Message} ");
        }
        return await response.Content.ReadAsStringAsync();
    }
}