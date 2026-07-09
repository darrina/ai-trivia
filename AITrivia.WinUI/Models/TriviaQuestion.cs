using System.Text.Json.Serialization;

namespace AITrivia.Models;

public class TriviaQuestion
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; } = "";

    [JsonPropertyName("question")]
    public string Question { get; set; } = "";

    [JsonPropertyName("answers")]
    public List<string> Answers { get; set; } = new();

    [JsonPropertyName("correctIndex")]
    public int CorrectIndex { get; set; }

    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; } = "easy";

    [JsonPropertyName("funFact")]
    public string FunFact { get; set; } = "";
}
