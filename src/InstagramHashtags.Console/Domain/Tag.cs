namespace InstagramHashtags.Console.Domain;

public partial struct Tag(string text)
{
    private string _text = text.ToLower().Trim();
    public string Text
    {
        readonly get => _text;
        set
        {
            if (!TagRegex().IsMatch(value))
            {
                throw new ArgumentException("Text can only contain alphanumeric characters and underscores.");
            }
            _text = value;
        }
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"^[a-zA-Z0-9_]+$")]
    private static partial System.Text.RegularExpressions.Regex TagRegex();
}