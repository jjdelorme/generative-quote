using Microsoft.AspNetCore.Mvc;
using GenerativeQuote.Models; // Assuming QuoteResponse is in the Models namespace

namespace GenerativeQuote.Controllers;

[ApiController]
[Route("[controller]")] // Routes requests to /quote
public class QuoteController : ControllerBase
{
    private readonly QuoteGenerator _generator;
    private readonly ILogger<QuoteController> _logger;

    public QuoteController(QuoteGenerator generator, ILogger<QuoteController> logger)
    {
        _generator = generator;
        _logger = logger;
    }

    [HttpGet("/random-quote")] // Matches the original route GET /random-quote
    public async Task<ActionResult<QuoteResponse>> GetRandomQuote([FromQuery] string prompt)
    {
        try
        {
            var result = await _generator.GetQuote(prompt);

            if (result == null)
            {
                // Consider returning a more specific message if possible
                return NotFound("Could not generate a quote based on the provided prompt.");
            }

            return Ok(result);
        }
        catch (Exception error)
        {
            _logger.LogError(error, "An error occurred while generating a random quote for prompt: {Prompt}", prompt);
            return Problem(detail: error.StackTrace, title: error.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}