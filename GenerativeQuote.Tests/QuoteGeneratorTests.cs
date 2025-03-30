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

            // Act
            var result = await quoteGenerator.GetQuote(prompt);

            // Assert
             mockPredictionServiceClient.Verify(c => c.GenerateContentAsync(It.Is<GenerateContentRequest>(request =>
                request.Contents.Count == 1 &&
                request.Contents[0].Parts.Count == 1 &&
                request.GenerationConfig.CandidateCount == 1 &&
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
