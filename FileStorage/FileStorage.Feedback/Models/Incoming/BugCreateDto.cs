namespace FileStorage.Feedback.Models.Incoming
{
    public class BugCreateDto
    {
        public string Text { get; set; } = null!;

        public int AppPartId { get; set; }
    }
}
