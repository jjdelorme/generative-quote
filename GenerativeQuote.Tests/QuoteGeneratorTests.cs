using Google.Cloud.AIPlatform.V1;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace GenerativeQuote.Tests;

public class QuoteGeneratorTests
{
    [Fact]
    public async Task GetQuote_ReturnsQuote()
    {
        // Arrange
        var options = new QuoteGeneratorOptions
        {
            ProjectId = "my-project",
            LocationId = "us-central1",
            ModelId = "my-model"
        };

        var mockOptions = new Mock<IOptions<QuoteGeneratorOptions>>();
        mockOptions.Setup(o => o.Value).Returns(options);

        var mockPredictionServiceClient = new Mock<PredictionServiceClient>();
        mockPredictionServiceClient.Setup(c => c.GenerateContentAsync(It.IsAny<GenerateContentRequest>(), null))
            .ReturnsAsync(new GenerateContentResponse
            {
                Candidates = { new Candidate { Content = new Content { Parts = { new Part { Text = "Hello, world!" } } } } }
            });

        var quoteGenerator = new QuoteGenerator(mockOptions.Object, mockPredictionServiceClient.Object);

        // Act
        var quote = await quoteGenerator.GetQuote("What is the meaning of life?");

        // Assert
        Assert.Equal("Hello, world!", quote);
    }
}
