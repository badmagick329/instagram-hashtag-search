namespace InstagramHashtags.Console.Domain;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ApiResponse
{
    [JsonProperty("data")]
    public Data Data { get; set; } = new Data();

    [JsonProperty("pagination_token")]
    public string? PaginationToken { get; set; }

    // [JsonIgnore]
    // public int PageNumber { get; set; }
    [JsonProperty("page_number")]
    public int PageNumber { get; set; } = 0;
}

public class Data
{
    [JsonProperty("additional_data")]
    public AdditionalData AdditionalData { get; set; } = new AdditionalData();


    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonProperty("items")]
    public JArray Items { get; set; } = [];
}

public class AdditionalData
{
    [JsonProperty("media_count")]
    public int? MediaCount { get; set; }

    [JsonProperty("profile_pic_url")]
    public string ProfilePicUrl { get; set; } = string.Empty;
}
