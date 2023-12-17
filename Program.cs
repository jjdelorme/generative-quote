/// <summary>
/// This application hosts a REST endpoint that generates a random quote from a fictional person.
/// </summary>

using GenerativeQuote;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var app = builder.Build();

app.MapGet("/random-quote", (string prompt) => GetQuote(prompt));

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

    VertexAIModelGenerator model = new VertexAIModelGenerator(projectId: projectId);

    var result = await model.GenerateTextAsync(PromptTemplate + prompt);

    return result.ToText();
}
