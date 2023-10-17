namespace FileStorage.Feedback.Models.Outcoming
{
    public class ErrorDto
    {
        public string Message { get; set; } = null!;

        public string? Exception { get; set; }
    }
}
