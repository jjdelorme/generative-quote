using Google.Api.Gax.Grpc;
using Google.Cloud.AIPlatform.V1;

namespace GenerativeQuote;

public class VertexAIModelGenerator
{
    private readonly HttpClient _httpClient;
    private readonly string _apiEndpoint;
    private readonly string _projectId;
    private readonly string _modelId;
    private readonly string _locationId;
    private readonly string _model;
    private readonly GenerationConfig _generationConfig;

    public VertexAIModelGenerator(
        string projectId, 
        string modelId = "gemini-1.0-pro-001", 
        string apiEndpoint = "us-central1-aiplatform.googleapis.com",         
        string locationId = "us-central1")
    {
        _projectId = projectId;
        _modelId = modelId;
        _locationId = locationId;
        _apiEndpoint = $"{_locationId}-aiplatform.googleapis.com";

        // `projects/{project}/locations/{location}/publishers/*/models/*`
        _model = $"projects/{_projectId}/locations/{_locationId}/publishers/google/models/{_modelId}";

        _generationConfig = new GenerationConfig() 
        { 
            CandidateCount = 1, 
            MaxOutputTokens = 256, 
            Temperature = 0.6f, 
            TopP = 1
        };
    }

    public async Task<string> GenerateTextAsync(string textPrompt)
    {
        // Create a prediction service client.
        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = _apiEndpoint
        }.Build();

        var content = new Content() { Role = "USER" };
        content.Parts.Add(new Part() { Text = textPrompt });

        GenerateContentRequest request = new GenerateContentRequest
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
