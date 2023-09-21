namespace FileStorage.Models.Db;

public partial class ElectedFile : BaseEntity
{
    public int FileId { get; set; }

    public int UserId { get; set; }

    public virtual File File { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
