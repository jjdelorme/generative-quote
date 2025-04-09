public class QuoteModel
{
    public string Quote { get; set; }
    public string Author { get; set; }
    
    // .NET 9 has a built in capability to generate the schema with the JsonSerializer.
    public static string Schema = @"
        {
            ""type"": ""object"",
            ""properties"": {
                ""author"": { ""type"": ""string"" },
                ""quote"": { ""type"": ""string"" },
            }
        }
    ";
}
