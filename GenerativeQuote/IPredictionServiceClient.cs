using Google.Cloud.AIPlatform.V1;

namespace GenerativeQuote;

public interface IPredictionServiceClient
{
    Task<GenerateContentResponse> GenerateContentAsync(GenerateContentRequest request);
}

public class PredictionServiceClientWrapper : IPredictionServiceClient
{
    private readonly PredictionServiceClient _client;

    public PredictionServiceClientWrapper(PredictionServiceClient client)
    {
        _client = client;
    }

    public async Task<GenerateContentResponse> GenerateContentAsync(GenerateContentRequest request)
    {
        return await _client.GenerateContentAsync(request);
    }
}
