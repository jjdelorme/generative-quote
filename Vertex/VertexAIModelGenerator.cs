using Google.Cloud.AIPlatform.V1;

namespace GenerativeQuote;

public class VertexAIModelGenerator
{
    private readonly string _apiEndpoint;
    private readonly string _model;
    private readonly GenerationConfig _generationConfig;

    public VertexAIModelGenerator(
        string projectId, 
        string modelId = "gemini-1.0-pro-001", 
        string locationId = "us-central1",
        float temperature = 0.6f)
    {
        // `projects/{project}/locations/{location}/publishers/*/models/*`
        _model = $"projects/{projectId}/locations/{locationId}/publishers/google/models/{modelId}";
        
        _apiEndpoint = $"{locationId}-aiplatform.googleapis.com";

        _generationConfig = new GenerationConfig() 
        { 
            CandidateCount = 1, 
            MaxOutputTokens = 256, 
            Temperature = temperature, 
            TopP = 1
        };
    }

    /// <summary>
    /// Invokes the Vertex AI Model to generate text.
    /// </summary>
    /// <param name="textPrompt"></param>
    /// <returns></returns>
    public async Task<string> GenerateTextAsync(string textPrompt)
    {
        // Create a prediction service client.
        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = _apiEndpoint
        }.Build();

        var content = new Content() { Role = "USER" };
        content.Parts.Add(new Part() { Text = textPrompt });

        var request = new GenerateContentRequest
        {
            Contents = { content, },
            GenerationConfig = _generationConfig,
            Model = _model,
        };
        
        var response = await predictionServiceClient.GenerateContentAsync(request);
        var text = response.Candidates.First().Content.Parts.First().Text;

        return text;
    }
}
