using System.Text.Json;
using Google.Apis.Auth.OAuth2;

namespace GenerativeQuote;

public class VertexAIModelGenerator
{
    private readonly string _apiEndpoint;
    private readonly string _projectId;
    private readonly string _modelId;
    private readonly string _locationId;

    public VertexAIModelGenerator(
        string projectId, 
        string modelId = "gemini-pro-vision", 
        string apiEndpoint = "us-central1-aiplatform.googleapis.com",         
        string locationId = "us-central1")
    {
        _apiEndpoint = apiEndpoint;
        _projectId = projectId;
        _modelId = modelId;
        _locationId = locationId;
    }

    public async Task<List<GenerateContentResponse>> GenerateTextAsync(string textPrompt)
    {
        using var client = new HttpClient();

        var url = $"https://{_apiEndpoint}/v1beta1/projects/{_projectId}/locations/{_locationId}/publishers/google/models/{_modelId}:streamGenerateContent";

        var accessToken = await GetAccessTokenAsync();
        
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        var requestBody = GenerateContentRequest.FromPrompt(textPrompt);

        // Create a JSON string from the requestBody
        var content = JsonContent.Create(requestBody);

        // Make the request
        var response = await client.PostAsync(url, content);

        // Check for errors
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error generating content: {response.StatusCode}");
        }
        
        var result = await JsonSerializer.DeserializeAsync<List<GenerateContentResponse>>(
            response.Content.ReadAsStream(), 
            options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        );
        
        return result;
    }

    private async Task<string> GetAccessTokenAsync()
    {
        // Use Application Default Credentials
        var credential = await GoogleCredential.GetApplicationDefaultAsync();

        // Create credential scoped to GCP APIs
        var scoped = credential.CreateScoped(
            new[] { "https://www.googleapis.com/auth/cloud-platform" });

        var accessToken = await scoped.UnderlyingCredential.GetAccessTokenForRequestAsync();

        return accessToken;
    }
}
