public class GenerateContentRequest
{
    public List<ContentInput> Contents { get; set; }
    public GenerationConfig GenerationConfig { get; set; }

    public static GenerateContentRequest FromPrompt(string prompt)
    {
        return new GenerateContentRequest
        {
            Contents = new List<ContentInput>
            {
                new ContentInput
                {
                    Role = "user",
                    Parts = new List<TextSegment>
                    {
                        new TextSegment
                        {
                            Text = prompt
                        }
                    }
                }
            },
            GenerationConfig = new GenerationConfig()
        };
    }
}

public class ContentInput
{
    public string Role { get; set; }
    public List<TextSegment> Parts { get; set; }
}

public class TextSegment
{
    public string Text { get; set; }
}

public class GenerationConfig
{
    public int MaxOutputTokens { get; set; } = 2048;
    public double Temperature { get; set; } = 0.6;
    public double TopP { get; set; } = 1;
    public int TopK { get; set; } = 32;
}
