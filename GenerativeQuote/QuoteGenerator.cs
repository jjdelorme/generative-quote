using Google.Cloud.AIPlatform.V1;

namespace GenerativeQuote;

public class QuoteGenerator
{
    private readonly string _model;
    private readonly string _apiEndpoint;

    public QuoteGenerator(
        string projectId, 
        string modelId = "gemini-1.0-pro-001", 
        string locationId = "us-central1")
    {
        if (string.IsNullOrEmpty(projectId))
            throw new Exception("Missing configuration variable: projectId");

        _model = $"projects/{projectId}/locations/{locationId}/publishers/google/models/{modelId}";
        
        _apiEndpoint = $"{locationId}-aiplatform.googleapis.com";
    }

    /// <summary>
    /// Returns a random quote from a fictional person.
    /// </summary>
    /// <example>"Generate a random quote from a fictional person."</example>
    public async Task<string> GetQuote(string prompt)
    {
        const string PromptTemplate = @"Goal: Create a creative, pithy random quote from a fictitious author.  Return this in JSON format and do not use markdown syntax.  Please do not include ```json.
    
            Few-Show Examples:

            [{
                'quote': 'If you never dream, you won\'t dream big',
                'author': 'Marty Rapinski'
            },
            {
            'quote': 'Holding the world in your hands is fairly wet',
            'author': 'Isaac Fortis'
            },
            {
            'quote': 'Where there\'s water, there's fish',
            'author': 'Theo Conway'
            }]

            Use the following text as the theme to generate a quote for: ";

        var result = await GenerateTextAsync(PromptTemplate + prompt);

        return result;
    }

    /// <summary>
    /// Invokes the Vertex AI Model to generate text.
    /// </summary>
    /// <param name="textPrompt">Your prompt</param>
    /// <param name="temperature">0.0 - 1.0 how creative the model should be</param>
    /// <returns></returns>
    private async Task<string> GenerateTextAsync(string textPrompt, float temperature = 0.6f)
    {
        var generationConfig = new GenerationConfig() 
        { 
            CandidateCount = 1, 
            MaxOutputTokens = 256, 
            Temperature = temperature, 
            TopP = 1
        };

        var content = new Content() { Role = "USER" };
        content.Parts.Add(new Part() { Text = textPrompt });

        var request = new GenerateContentRequest
        {
            Contents = { content, },
            GenerationConfig = generationConfig,
            Model = _model,
        };

        // Create a prediction service client.
        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = _apiEndpoint
        }.Build();

        GenerateContentResponse response = await predictionServiceClient.GenerateContentAsync(request);
        var text = response.Candidates.First().Content.Parts.First().Text;

        return text;
    }
}
