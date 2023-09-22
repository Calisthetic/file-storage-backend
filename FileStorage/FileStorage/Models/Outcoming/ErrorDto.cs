using System.Text.Json.Serialization;

namespace FileStorage.Models.Outcoming
{
    public class ErrorDto
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = null!;
        [JsonPropertyName("exception")]
        public string? Exception { get; set; }
    }
}
