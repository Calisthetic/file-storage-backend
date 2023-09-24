using System.Text.Json.Serialization;

namespace FileStorage.Models.Outcoming
{
    public class UserAuthResultDto
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = null!;
    }
}
