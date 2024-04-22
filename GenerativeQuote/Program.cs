/// <summary>
/// This application hosts a REST endpoint that generates a random quote from a fictional person.
/// </summary>

using GenerativeQuote;
using Google.Cloud.Logging.Console;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(o => o.AddDefaultPolicy(builder => {
    builder.AllowAnyMethod();
    builder.AllowAnyHeader();
    builder.AllowAnyOrigin();
}));
builder.Services.AddHttpClient();
builder.Services.Configure<QuoteGeneratorOptions>(
    builder.Configuration.GetSection(QuoteGeneratorOptions.QuoteGenerator));
builder.Services.AddSingleton<QuoteGenerator>();

if (builder.Environment.IsProduction())
{
    // When running in production (Cloud Run) add Google logging formatter.
    builder.Logging.AddGoogleCloudConsole();
}

var config = builder.Configuration;
var app = builder.Build();

var log = app.Logger;

app.UseCors();

app.MapGet("/random-quote", async (QuoteGenerator generator, string prompt) => 
{
    try
    {
        var result = await generator.GetQuote(prompt);
        
        if (result == null)
        {
            return Results.NotFound("No response from Vertex Search");
        }
        else
        {
            return Results.Ok(result);
        }
    }
    catch (Exception error)
    {
        log.LogError("An error occurred while generating a random quote for prompt: {0},\n{1}", 
            error.Message, error.StackTrace);
        return Results.Problem(detail: error.StackTrace, title: error.Message, statusCode: 500);
    }
});

app.Run();
