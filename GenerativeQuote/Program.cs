/// <summary>
/// This application hosts a REST endpoint that generates a random quote from a fictional person.
/// </summary>

using GenerativeQuote.Models; // Add if QuoteResponse is used elsewhere or needed for configuration
using GenerativeQuote;
using Google.Cloud.Logging.Console;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(o => o.AddDefaultPolicy(builder => {
    builder.AllowAnyMethod();
    builder.AllowAnyHeader();
    builder.AllowAnyOrigin();
}));

builder.Services.Configure<QuoteGeneratorOptions>(
    builder.Configuration.GetSection(QuoteGeneratorOptions.QuoteGenerator));

builder.Services.AddPredictionServiceClient(client => {
    var options = QuoteGeneratorOptions.FromConfiguration(builder.Configuration);
    client.Endpoint = $"{options.LocationId}-aiplatform.googleapis.com";
});

builder.Services.AddSingleton<QuoteGenerator>();
builder.Services.AddScoped<IPredictionServiceClient, PredictionServiceClientWrapper>();

if (builder.Environment.IsProduction())
{
    // When running in production (Cloud Run) add Google logging formatter.
    builder.Logging.AddGoogleCloudConsole();
}

// Add services for controllers
builder.Services.AddControllers();

var config = builder.Configuration;
var app = builder.Build();

app.UseCors();

// Map controller endpoints
app.MapControllers();

app.Run();

