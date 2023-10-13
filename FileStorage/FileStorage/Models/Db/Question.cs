namespace FileStorage.Models.Db
{
    public class Question : BaseEntity
    {
        public int UserId { get; set; }

        public string Text { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public string? Answer { get; set; }

        public DateTime? RespondedAt { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
