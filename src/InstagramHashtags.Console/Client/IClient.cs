namespace InstagramHashtags.Console.Client;

using InstagramHashtags.Console.Domain;
public interface IClient
{
    public Task<string> SearchTag(Tag tag);
}