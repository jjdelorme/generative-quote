using Google.Cloud.AIPlatform.V1;

namespace GenerativeQuote;

public class GenerationConfigOptions
{
    public int CandidateCount { get; set; }
    public int MaxOutputTokens { get; set; }
    public float Temperature { get; set; }
    public float TopP { get; set; }
}

public class QuoteGeneratorOptions
{
    public const string QuoteGenerator = "QuoteGenerator";
    
    public string ProjectId { get; set; } = "";
    public string ModelId { get; set; } = "";
    public string LocationId { get; set; } = "";
    public GenerationConfigOptions GenerationConfig { get; set; } = new();

    public static QuoteGeneratorOptions FromConfiguration(IConfiguration config)
    {
        var options = new QuoteGeneratorOptions();
        config.GetSection(QuoteGenerator).Bind(options);
        return options;
    }
}
