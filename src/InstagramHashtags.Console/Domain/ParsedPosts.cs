namespace InstagramHashtags.Console.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class PostUser
{
    public string FullName { get; private set; }
    public string Username { get; private set; }

    public PostUser(string fullName, string username)
    {
        FullName = fullName;
        Username = username;
    }

    public override string ToString() => $"{FullName}({Username})";
}


public class ParsedPost
{
    public string AccessibilityCaption { get; private set; }
    public string Code { get; private set; }
    public bool CaptionIsEdited { get; private set; }
    public int CommentCount { get; private set; }
    public string CommercialityStatus { get; private set; }
    public bool HasHighRiskGenAiInformTreatment { get; private set; }
    public bool HasSharedToFb { get; private set; }
    public bool IsPaidPartnership { get; private set; }
    public bool IsVideo { get; private set; }
    public int LikeCount { get; private set; }
    public List<PostUser>? Likers { get; private set; }
    public PostUser User { get; private set; }
    public PostUser Owner { get; private set; }
    public List<string> TopLikers { get; private set; }
    public List<string> Hashtags { get; private set; }
    public int CreatedAtUtcTimestamp { get; private set; }
    public DateTime CreatedAtUtc => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUtcTimestamp).UtcDateTime;
    public DateTime CreatedAtIslamabadTime => CreatedAtUtc.AddHours(5);
    public string CaptionText { get; private set; }
    public string LocationAddress { get; private set; }
    public string LocationCity { get; private set; }
    public string LocationName { get; private set; }
    public string LocationExternalSource { get; private set; }
    public string LocationLat { get; private set; }
    public string LocationLng { get; private set; }


    public ParsedPost(JObject jObject)
    {
        AccessibilityCaption = jObject["accessibility_caption"]?.ToString() ?? "";
        Code = jObject["code"]?.ToString() ?? "";
        CaptionIsEdited = jObject["caption_is_edited"]?.ToObject<bool>() ?? false;
        CommentCount = jObject["comment_count"]?.ToObject<int>() ?? 0;
        CommercialityStatus = jObject["commerciality_status"]?.ToString() ?? "";
        HasHighRiskGenAiInformTreatment = jObject["has_high_risk_gen_ai_inform_treatment"]?.ToObject<bool>() ?? false;
        HasSharedToFb = jObject["has_shared_to_fb"]?.ToObject<bool>() ?? false;
        IsPaidPartnership = jObject["is_paid_partnership"]?.ToObject<bool>() ?? false;
        IsVideo = jObject["is_video"]?.ToObject<bool>() ?? false;
        LikeCount = jObject["like_count"]?.ToObject<int>() ?? 0;
        Likers = jObject["likers"]?.ToObject<List<PostUser>>();
        User = new PostUser(jObject["user"]?["full_name"]?.ToString() ?? "", jObject["user"]?["username"]?.ToString() ?? "");
        Owner = new PostUser(jObject["owner"]?["full_name"]?.ToString() ?? "", jObject["owner"]?["username"]?.ToString() ?? "");
        TopLikers = jObject["top_likers"]?.ToObject<List<string>>() ?? new List<string>();
        Hashtags = jObject["caption"]?["hashtags"]?.ToObject<List<string>>() ?? new List<string>();
        CreatedAtUtcTimestamp = jObject["caption"]?["created_at_utc"]?.ToObject<int>() ?? 0;
        CaptionText = jObject["caption"]?["text"]?.ToString() ?? "";
        LocationAddress = jObject["location"]?["address"]?.ToString() ?? "";
        LocationCity = jObject["location"]?["city"]?.ToString() ?? "";
        LocationName = jObject["location"]?["name"]?.ToString() ?? "";
        LocationExternalSource = jObject["location"]?["external_source"]?.ToString() ?? "";
        LocationLat = jObject["location"]?["lat"]?.ToString() ?? "";
        LocationLng = jObject["location"]?["lng"]?.ToString() ?? "";
    }

    public override string ToString()
    {
        string likersString = Likers?.Select(l => l.ToString()).Aggregate((acc, l) => acc + ", " + l) ?? "";
        string topLikersString = TopLikers.Aggregate((acc, l) => acc + ", " + l);
        string hashTagsString = Hashtags.Aggregate((acc, l) => acc + ", " + l);

        return (
            $"AccessibilityCaption: {AccessibilityCaption}{Environment.NewLine}" +
            $"Code: {Code}{Environment.NewLine}" +
            $"Possible IG URL: https://www.instagram.com/p/{Code}{Environment.NewLine}" +
            $"CaptionIsEdited: {CaptionIsEdited}{Environment.NewLine}" +
            $"CommentCount: {CommentCount}{Environment.NewLine}" +
            $"CommercialityStatus: {CommercialityStatus}{Environment.NewLine}" +
            $"HasHighRiskGenAiInformTreatment: {HasHighRiskGenAiInformTreatment}{Environment.NewLine}" +
            $"HasSharedToFb: {HasSharedToFb}{Environment.NewLine}" +
            $"IsPaidPartnership: {IsPaidPartnership}{Environment.NewLine}" +
            $"IsVideo: {IsVideo}{Environment.NewLine}" +
            $"LikeCount: {LikeCount}{Environment.NewLine}" +
            $"Likers: {likersString}{Environment.NewLine}" +
            $"User: {User}{Environment.NewLine}" +
            $"Owner: {Owner}{Environment.NewLine}" +
            $"TopLikers: {topLikersString}{Environment.NewLine}" +
            $"Hashtags: {hashTagsString}{Environment.NewLine}" +
            $"CreatedAtUtc: {CreatedAtUtc}{Environment.NewLine}" +
            $"CreatedAtIslamabadTime: {CreatedAtIslamabadTime}{Environment.NewLine}" +
            $"CaptionText: {CaptionText}{Environment.NewLine}" +
            $"LocationAddress: {LocationAddress}{Environment.NewLine}" +
            $"LocationCity: {LocationCity}{Environment.NewLine}" +
            $"LocationName: {LocationName}{Environment.NewLine}" +
            $"LocationExternalSource: {LocationExternalSource}{Environment.NewLine}" +
            $"LocationLat: {LocationLat}{Environment.NewLine}" +
            $"LocationLng: {LocationLng}{Environment.NewLine}"
        );
    }
}

public class ParsedPosts
{
    public List<ParsedPost> Posts { get; set; } = [];
    public ParsedPosts(List<ApiResponse> apiResponses)
    {
        foreach (ApiResponse apiResponse in apiResponses)
        {
            foreach (JObject jObject in apiResponse.Data.Items)
            {
                ParsedPost parsedPost = new(jObject);
                Posts.Add(parsedPost);
            }
        }
    }
    public ParsedPosts(ApiResponse apiResponse)
    : this([apiResponse]) { }
}