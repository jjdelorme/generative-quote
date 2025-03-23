public class QuoteModel
{
    public string Quote { get; set; }
    public string Author { get; set; }
    
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
