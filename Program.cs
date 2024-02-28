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

if (builder.Environment.IsProduction())
{
    // When running in production (Cloud Run) add Google logging formatter.
    builder.Logging.AddGoogleCloudConsole();
}

var config = builder.Configuration;
var app = builder.Build();

var log = app.Logger;

app.UseCors();

app.MapGet("/random-quote", async (string prompt) => 
{
    try
    {
        var result = await GetQuote(prompt);
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

/// <summary>
/// Returns a random quote from a fictional person.
/// </summary>
/// <example>"Generate a random quote from a fictional person."</example>
async Task<string> GetQuote(string prompt)
{
    const string PromptTemplate = @"Goal: Create a creative, pithy random quote from a fictitious author.  Return this in JSON format and do not use markdown syntax.  Please do not include ```json.
   
        Few-Show Examples:

        [{
            'quote': 'If you never dream, you won\'t dream big',
            'author': 'Marty Rapinski'
        },
        {
        'quote': 'Holding the world in your hands is fairly wet',
        'author': 'Isaac Fortis'
        },
        {
        'quote': 'Where there\'s water, there's fish',
        'author': 'Theo Conway'
        }]

        Use the following text as the theme to generate a quote for: ";
 
    string projectId = config["projectId"];

    if (string.IsNullOrEmpty(projectId))
        throw new Exception("Missing configuration variable: projectId");

    var httpClient = app.Services.GetRequiredService<HttpClient>();

    VertexAIModelGenerator model = new VertexAIModelGenerator(httpClient, projectId: projectId);

    var result = await model.GenerateTextAsync(PromptTemplate + prompt);

    return result.ToText();
}
