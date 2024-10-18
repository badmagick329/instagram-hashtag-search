namespace InstagramHashtags.Console.Client;

using System;
using InstagramHashtags.Console.Domain;
using Newtonsoft.Json;


public class IgClient(string host, string key) : IClient
{
    private HttpClient HttpClient { get; set; } = new HttpClient();
    private string ApiHost { get; set; } = host;
    private string ApiKey { get; set; } = key;

    public int MaxPages { get; set; } = 2;

    /// <summary>
    /// Searches for the specified tag and returns the response as a string.
    /// </summary>
    /// <param name="tag">The tag to search for.</param>
    /// <returns name="apiResponses">A list of ApiResponse objects.</returns>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="TaskCanceledException"></exception>
    public async Task<List<ApiResponse>> SearchTag(Tag tag)
    {
        string firstResponse = await MakeRequest(tag, null);
        int currentPage = 1;
        ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(firstResponse)
            ?? throw new ArgumentNullException("Error deserializing response.");

        apiResponse.PageNumber = currentPage;
        List<ApiResponse> apiResponses = [apiResponse];
        int resultsSoFar = apiResponses.Select(r => r.Data.Count).Sum();
        Console.WriteLine($"Results so far: {resultsSoFar}");
        int retries = 0;
        int maxRetries = 2;

        while (apiResponse.PaginationToken is not null && currentPage < MaxPages)
        {
            try
            {
                string response = await MakeRequest(tag, apiResponse.PaginationToken);
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response)
                    ?? throw new ArgumentNullException("Error deserializing response.");
                currentPage++;
                apiResponse.PageNumber = currentPage;
                apiResponses.Add(apiResponse);
                resultsSoFar = apiResponses.Select(r => r.Data.Count).Sum();
                Console.WriteLine($"Results so far: {resultsSoFar}");
            }
            catch (Exception e)
            {
                retries++;
                Console.WriteLine($"Error during request\n{e.Message}\nRetrying {retries}/{maxRetries}");
                if (retries > maxRetries)
                {
                    Console.WriteLine("Too many retries. Exiting.");
                    break;
                }
                continue;
            }
        }
        return apiResponses;
    }

    private async Task<string> MakeRequest(Tag tag, string? token)
    {
        string uri = $"https://instagram-scraper-api2.p.rapidapi.com/v1/hashtag?hashtag={tag.Text}";
        if (token is not null)
        {
            uri += $"&pagination_token={token}";
        }
        Console.WriteLine($"Making request...\n{uri}");
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(uri),
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
            return string.Empty;
        }
        return await response.Content.ReadAsStringAsync();
    }
}