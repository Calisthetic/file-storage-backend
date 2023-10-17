namespace FileStorage.Feedback.Models.Outcoming
{
    public class QuestionInfoDto
    {
        public int Id { get; set; }

        public string Text { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public string Answer { get; set; } = null!;

        public DateTime? RespondedAt { get; set; }
    }
}
