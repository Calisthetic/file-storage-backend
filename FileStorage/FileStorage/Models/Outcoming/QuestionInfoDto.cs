using FileStorage.Models.Db;

namespace FileStorage.Models.Outcoming
{
    public class QuestionInfoDto : BaseEntity
    {
        public string Text { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public string Answer { get; set; } = null!;

        public DateTime? RespondedAt { get; set; }
    }
}
