namespace GenerativeQuote.Models;

/// <summary>
/// Represents the response containing the generated quote.
/// </summary>
public class QuoteResponse
{
    public string? Quote { get; set; }
    public string? Author { get; set; }
}