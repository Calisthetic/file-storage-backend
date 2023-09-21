namespace FileStorage.Models.Db
{
    public class User : BaseEntity
    {
        public User()
        {
            Emails = new HashSet<Email>();
        }

        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? InvitedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? PrimaryEmail { get; set; }

        public virtual ICollection<Email> Emails { get; set;}
    }
}
