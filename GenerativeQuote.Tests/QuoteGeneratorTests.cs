using Google.Cloud.AIPlatform.V1;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using Xunit;

namespace GenerativeQuote.Tests;

public class QuoteGeneratorTests
{
    [Fact]
    public async Task GetQuote_ReturnsQuote_WhenModelReturnsValidJson()
    {
        // Arrange
        var options = new QuoteGeneratorOptions { ProjectId = "test-project", LocationId = "us-central1", ModelId = "test-model" };
        var mockPredictionServiceClient = new Mock<IPredictionServiceClient>();
        var expectedQuote = new QuoteModel { Author = "Test Author", Quote = "Test Quote" };
        var json = System.Text.Json.JsonSerializer.Serialize(expectedQuote);

        var candidate = new Candidate()
        {
            Content = new Content() { Parts = { new Part() { Text = json } } },
            FinishReason = Candidate.Types.FinishReason.Stop
        };

        mockPredictionServiceClient.Setup(x => x.GenerateContentAsync(It.IsAny<GenerateContentRequest>()))
            .ReturnsAsync(new GenerateContentResponse { Candidates = { candidate } });

        var quoteGenerator = new QuoteGenerator(Options.Create(options), mockPredictionServiceClient.Object);

        // Act
        var quote = await quoteGenerator.GetQuote("test theme");

        // Assert
        Assert.Equal(expectedQuote.Author, quote.Author);
        Assert.Equal(expectedQuote.Quote, quote.Quote);
    }


    [Fact]
    public async Task GetQuote_ThrowsException_WhenModelReturnsInvalidJson()
    {
        // Arrange
        var options = new QuoteGeneratorOptions { ProjectId = "test-project", LocationId = "us-central1", ModelId = "test-model" };
        var mockPredictionServiceClient = new Mock<IPredictionServiceClient>();

        var candidate = new Candidate()
        {
            Content = new Content() { Parts = { new Part() { Text = "invalid json" } } },
            FinishReason = Candidate.Types.FinishReason.Stop
        };

        mockPredictionServiceClient.Setup(x => x.GenerateContentAsync(It.IsAny<GenerateContentRequest>()))
            .ReturnsAsync(new GenerateContentResponse { Candidates = { candidate } });

        var quoteGenerator = new QuoteGenerator(Options.Create(options), mockPredictionServiceClient.Object);

        // Act & Assert
        await Assert.ThrowsAsync<JsonException>(() => quoteGenerator.GetQuote("test theme"));
    }

    [Fact]
    public async Task GetQuote_ThrowsException_WhenModelReturnsNoResponse()
    {
        // Arrange
        var options = new QuoteGeneratorOptions { ProjectId = "test-project", LocationId = "us-central1", ModelId = "test-model" };
        var mockPredictionServiceClient = new Mock<IPredictionServiceClient>();
        mockPredictionServiceClient.Setup(x => x.GenerateContentAsync(It.IsAny<GenerateContentRequest>()))
            .ReturnsAsync(new GenerateContentResponse { Candidates = { } });

        var quoteGenerator = new QuoteGenerator(Options.Create(options), mockPredictionServiceClient.Object);

        // Act & Assert
        await Assert.ThrowsAsync<QuoteGenerator.QuoteGeneratorException>(() => quoteGenerator.GetQuote("test theme"));
    }

    [Fact]
    public async Task GetQuote_ThrowsException_WhenModelReturnsEmptyText()
    {
        // Arrange
        var options = new QuoteGeneratorOptions { ProjectId = "test-project", LocationId = "us-central1", ModelId = "test-model" };
        var mockPredictionServiceClient = new Mock<IPredictionServiceClient>();

        var candidate = new Candidate()
        {
            Content = new Content() { Parts = { new Part() { Text = "" } } },
            FinishReason = Candidate.Types.FinishReason.Stop
        };

        mockPredictionServiceClient.Setup(x => x.GenerateContentAsync(It.IsAny<GenerateContentRequest>()))
            .ReturnsAsync(new GenerateContentResponse { Candidates = { candidate } });

        var quoteGenerator = new QuoteGenerator(Options.Create(options), mockPredictionServiceClient.Object);

        // Act & Assert
        await Assert.ThrowsAsync<QuoteGenerator.QuoteGeneratorException>(() => quoteGenerator.GetQuote("test theme"));
    }

    [Fact]
    public async Task GetQuote_ThrowsException_WhenModelStopsEarly()
    {
        // Arrange
        var options = new QuoteGeneratorOptions { ProjectId = "test-project", LocationId = "us-central1", ModelId = "test-model" };
        var mockPredictionServiceClient = new Mock<IPredictionServiceClient>();

        var candidate = new Candidate()
        {
            Content = new Content() { Parts = { new Part() { Text = "some text" } } },
            FinishReason = Candidate.Types.FinishReason.Safety,
            FinishMessage = "Safety"
        };

        mockPredictionServiceClient.Setup(x => x.GenerateContentAsync(It.IsAny<GenerateContentRequest>()))
            .ReturnsAsync(new GenerateContentResponse { Candidates = { candidate } });

        var quoteGenerator = new QuoteGenerator(Options.Create(options), mockPredictionServiceClient.Object);

        // Act & Assert
        await Assert.ThrowsAsync<QuoteGenerator.QuoteGeneratorException>(() => quoteGenerator.GetQuote("test theme"));
    }

    [Fact]
    public void QuoteGenerator_ThrowsException_WhenProjectIdIsMissing()
    {
        //Arrange
        var options = new QuoteGeneratorOptions { LocationId = "us-central1", ModelId = "test-model" };
        var mockPredictionServiceClient = new Mock<IPredictionServiceClient>();

        //Act & Assert
        Assert.Throws<Exception>(() => new QuoteGenerator(Options.Create(options), mockPredictionServiceClient.Object));
    }
}
