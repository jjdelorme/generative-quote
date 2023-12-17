/// <summary>
/// This application hosts a REST endpoint that generates a random quote from a fictional person.
/// </summary>

using GenerativeQuote;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var app = builder.Build();

app.MapGet("/random-quote", (string text) => GetQuote(text));

app.Run();

/// <summary>
/// Returns a random quote from a fictional person.
/// </summary>
/// <example>"Generate a random quote from a fictional person."</example>
async Task<string> GetQuote(string text)
{
    const string Prompt = @"Goal: Create a creative, pithy random quote from a fictious author.
    
    Few-Show Examples:

    1. `If you never dream, you won't dream big` ~ Marty Rapinski
    2. `Holding the world in your hands is fairly wet` ~ Isaac Fortis
    3. `Where there's water, there's fish` ~ Theo Conway

    Use the following text as the theme to generate a quote for: 
    ";

    string projectId = config["projectId"];

    if (string.IsNullOrEmpty(projectId))
        throw new Exception("Missing configuration variable: projectId");

    VertexAIModelGenerator model = new VertexAIModelGenerator(projectId: projectId);

    var result = await model.GenerateTextAsync(Prompt + text);

    return result.ToText();
}
