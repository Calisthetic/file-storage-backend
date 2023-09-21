namespace FileStorage.Models.Db
{
    public class Email : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int UserId { get; set; }
        public bool IsVerify { get; set; }

        public virtual User User { get; set; }
    }
}
