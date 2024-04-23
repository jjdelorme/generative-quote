namespace GenerativeQuote;

public class QuoteGeneratorOptions
{
    public const string QuoteGenerator = "QuoteGenerator";
    
    public string ProjectId { get; set; }
    public string ModelId { get; set; }
    public string LocationId { get; set; }

    public static QuoteGeneratorOptions FromConfiguration(ConfigurationManager config)
    {
        return config.GetSection(QuoteGenerator).Get<QuoteGeneratorOptions>();
    }
}
