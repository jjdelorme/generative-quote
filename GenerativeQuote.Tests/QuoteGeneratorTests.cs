using System.Text.Json;
using Google.Cloud.AIPlatform.V1;
using Microsoft.Extensions.Options;
using Moq;
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
            ModelId = "text-bison-001"
        };

        var mockPredictionServiceClient = new Mock<PredictionServiceClient>();
        mockPredictionServiceClient
            .Setup(x => x.GenerateContentAsync(It.IsAny<GenerateContentRequest>(), null))
            .ReturnsAsync(new GenerateContentResponse
            {
                Candidates = { new Candidate { Content = new Content { Parts = { new Part { Text = "{\"quote\":\"Hello, world!\",\"author\":\"John Doe\"}" } } } } }
            });

        var quoteGenerator = new QuoteGenerator(Options.Create(options), mockPredictionServiceClient.Object);

        // Act
        var quote = await quoteGenerator.GetQuote("Hello, world!");

        // Assert
        Assert.Equal("Hello, world!", quote.Quote);
        Assert.Equal("John Doe", quote.Author);
    }

    [Fact]
    public void GetQuote_ThrowsException_WhenProjectIdMissing()
    {
        // Arrange
        var options = new QuoteGeneratorOptions
        {
            LocationId = "us-central1",
            ModelId = "text-bison-001"
        };

        var mockPredictionServiceClient = new Mock<PredictionServiceClient>();

        // Act and Assert
        Assert.Throws<Exception>(() => new QuoteGenerator(Options.Create(options), mockPredictionServiceClient.Object));
    }
}
