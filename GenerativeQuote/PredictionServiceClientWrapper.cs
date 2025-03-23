using Google.Cloud.AIPlatform.V1;
using System.Threading.Tasks;

namespace GenerativeQuote
{
    public interface IPredictionServiceClient
    {
        Task<GenerateContentResponse> GenerateContentAsync(GenerateContentRequest request);
    }

    /// <summary>
    /// This wrapper provides an abstraction that makes the code more testable.
    /// </summary>
    public class PredictionServiceClientWrapper : IPredictionServiceClient
    {
        private PredictionServiceClient _client;

        public PredictionServiceClientWrapper(PredictionServiceClient client)
        {
            _client = client;
        }

        public async Task<GenerateContentResponse> GenerateContentAsync(GenerateContentRequest request)
        {
            return await _client.GenerateContentAsync(request);
        }
    }
}
