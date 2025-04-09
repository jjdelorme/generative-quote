using System.Text.Json;
using Google.Cloud.AIPlatform.V1;
using Microsoft.Extensions.Options;

namespace GenerativeQuote;

public class QuoteGenerator
{
    private readonly QuoteGeneratorOptions _options;
    private readonly string _model;
    private readonly IPredictionServiceClient _predictionServiceClient;

    /// <summary>
    /// Goal instructions for the LLM.
    /// </summary>
    private const string PromptGoal = "Create a creative, pithy random quote from a fictitious author";

    private static readonly GenerationConfig GenerationConfig = new() 
    { 
        CandidateCount = 1, 
        MaxOutputTokens = 256, 
        Temperature = 0.6f, 
        TopP = 1,
        ResponseMimeType = "application/json"
    };


    public QuoteGenerator(IOptions<QuoteGeneratorOptions> options, 
        IPredictionServiceClient predictionServiceClient)
    {
        _options = options.Value;
        
        if (string.IsNullOrEmpty(_options.ProjectId))
            throw new Exception("Missing configuration variable: projectId");

        _model = $"projects/{_options.ProjectId}/locations/{_options.LocationId}/publishers/google/models/{_options.ModelId}";
        
        _predictionServiceClient = predictionServiceClient;
    }

    /// <summary>
    /// Returns a random quote from a fictional person.
    /// </summary>
    /// <example>"Generate a random quote from a fictional person."</example>
    public async Task<QuoteModel> GetQuote(string theme)
    {
        var prompt = $@"
            Goal: {PromptGoal}.  Use the following JSON schema for your response: {QuoteModel.Schema}
            Use the following text as the theme to generate a quote for: {theme}
        ";

        var response = await GenerateTextAsync(prompt);

        var quote = JsonSerializer.Deserialize<QuoteModel>(response, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return quote;
    }

    /// <summary>
    /// Invokes the Vertex AI Model to generate text.
    /// </summary>
    /// <param name="textPrompt">Your prompt</param>
    /// <returns></returns>
    private async Task<string> GenerateTextAsync(string textPrompt)
    {
        var content = new Content() { Role = "USER" };
        content.Parts.Add(new Part() { Text = textPrompt });

        var request = new GenerateContentRequest
        {
            Contents = { content, },
            GenerationConfig = GenerationConfig,
            Model = _model,
        };

        try
        {
            var response = await _predictionServiceClient.GenerateContentAsync(request);

            if (response.Candidates?.Count() <= 0)
                throw new QuoteGeneratorException("No response from the the model.");

            var candidate = response.Candidates.First();

            if (candidate.FinishReason != Candidate.Types.FinishReason.Stop) 
                throw new QuoteGeneratorException(
                    $"Model stopped with {candidate.FinishReason}: {candidate.FinishMessage}");
            
            var text = candidate.Content.Parts.First().Text;

            if (string.IsNullOrEmpty(text))
                throw new QuoteGeneratorException("Empty text response from the the model.");

            return text.Trim();
        }
        catch (Exception e)
        {
            throw new QuoteGeneratorException(
                $"An error occurred while generating text: {e.Message}", e);
        }
    }

    public class QuoteGeneratorException : Exception
    {
        public QuoteGeneratorException(string message, Exception innerException = null) : 
            base(message, innerException) { }
    }
}
