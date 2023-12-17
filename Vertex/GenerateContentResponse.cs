namespace GenerativeQuote;

using System.Text;

public class GenerateContentResponse
{
    public List<Candidate> Candidates { get; set; }
    public UsageMetadata UsageMetadata { get; set; }
}

public class Candidate
{
    public Content Content { get; set; }
    public string FinishReason { get; set; }
    public List<SafetyRating> SafetyRatings { get; set; }
    public CitationMetadata CitationMetadata { get; set; }
}

public class Content
{
    public List<Part> Parts { get; set; }
}

public class Part
{
    public string Text { get; set; }
}

public class SafetyRating
{
    public string Category { get; set; }
    public string Probability { get; set; }
    public bool Blocked { get; set; }
}

public class CitationMetadata
{
    public List<Citation> Citations { get; set; }
}

public class Citation
{
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public string Uri { get; set; }
    public string Title { get; set; }
    public string License { get; set; }
    public PublicationDate PublicationDate { get; set; }
}

public class PublicationDate
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
}

public class UsageMetadata
{
    public int PromptTokenCount { get; set; }
    public int CandidatesTokenCount { get; set; }
    public int TotalTokenCount { get; set; }
}

public static class GenerateContentResponseExtensions
{
    public static string ToText(this List<GenerateContentResponse> responses)
    {
        var sb = new StringBuilder();
        foreach (var response in responses)
        {
            foreach (var candidate in response.Candidates)
            {
                foreach (var part in candidate.Content.Parts)
                {
                    sb.Append(part.Text);
                }
            }
        }
        return sb.ToString();
    }
}
