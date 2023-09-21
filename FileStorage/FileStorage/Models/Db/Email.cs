namespace FileStorage.Models.Db;

public class Email : BaseEntity
{
    public string Name { get; set; } = null!;

    public int UserId { get; set; }

    public bool IsVerify { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
