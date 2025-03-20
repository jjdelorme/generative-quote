using GenerativeQuote;
using Google.Cloud.AIPlatform.V1;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace GenerativeQuote.Tests
{
    public class QuoteGeneratorTests
    {
        [Fact]
        public async Task GetQuote_ValidPrompt_ReturnsQuote()
        {
            // Arrange
            var mockOptions = new Mock<IOptions<QuoteGeneratorOptions>>();
            mockOptions.Setup(o => o.Value).Returns(new QuoteGeneratorOptions
            {
                ProjectId = "test-project",
                ModelId = "test-model",
                LocationId = "test-location"
            });

            var mockPredictionServiceClient = new Mock<IPredictionServiceClient>();
            mockPredictionServiceClient.Setup(c => c.GenerateContentAsync(It.IsAny<GenerateContentRequest>()))
                .ReturnsAsync(new GenerateContentResponse
                {
                    Candidates = { new Candidate { Content = new Content { Parts = { new Part { Text = "{\"author\": \"Test Author\", \"quote\": \"Test Quote\"}" } } } } }
                });

            var quoteGenerator = new QuoteGenerator(mockOptions.Object, mockPredictionServiceClient.Object);
            var prompt = "Test Prompt";
            var expectedModel = $"projects/test-project/locations/test-location/publishers/google/models/test-model";
            var expectedPrompt = @"Goal: Create a creative, pithy random quote from a fictitious author.  Use the following JSON schema:
            {
                ""type"": ""object"",
                ""properties"": {
                    ""author"": { ""type"": ""string"" },
                    ""quote"": { ""type"": ""string"" },
                }
            }

            Use the following text as the theme to generate a quote for: " + prompt;

            // Act
            var result = await quoteGenerator.GetQuote(prompt);

            // Assert
             mockPredictionServiceClient.Verify(c => c.GenerateContentAsync(It.Is<GenerateContentRequest>(request =>
                request.Model == expectedModel &&
                request.Contents.Count == 1 &&
                request.Contents[0].Parts.Count == 1 &&
                request.Contents[0].Parts[0].Text == expectedPrompt &&
                request.GenerationConfig.CandidateCount == 1 &&
                request.GenerationConfig.MaxOutputTokens == 256 &&
                request.GenerationConfig.Temperature == 0.6f &&
                request.GenerationConfig.TopP == 1 &&
                request.GenerationConfig.ResponseMimeType == "application/json"
            )), Times.Once);
        }

        [Fact]
        public void Constructor_MissingProjectId_ThrowsException()
        {
            // Arrange
            var mockOptions = new Mock<IOptions<QuoteGeneratorOptions>>();
            mockOptions.Setup(o => o.Value).Returns(new QuoteGeneratorOptions
            {
                ProjectId = "", // Missing ProjectId
                ModelId = "test-model",
                LocationId = "test-location"
            });

            var mockPredictionServiceClient = new Mock<IPredictionServiceClient>();

            // Act & Assert
            Assert.Throws<System.Exception>(() => new QuoteGenerator(mockOptions.Object, mockPredictionServiceClient.Object));
        }
    }
}
