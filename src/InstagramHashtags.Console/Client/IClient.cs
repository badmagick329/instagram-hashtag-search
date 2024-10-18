namespace InstagramHashtags.Console.Client;

using InstagramHashtags.Console.Domain;

public interface IClient
{
    public Task<List<ApiResponse>> SearchTag(Tag tag);
}