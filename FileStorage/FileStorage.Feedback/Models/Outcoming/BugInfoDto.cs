namespace FileStorage.Feedback.Models.Outcoming
{
    public class BugInfoDto
    {
        public string Text { get; set; } = null!;

        public string BugStatus { get; set; } = null!;

        public string AppPart { get; set; } = null!;
    }
}
