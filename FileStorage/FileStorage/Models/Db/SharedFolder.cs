namespace FileStorage.Models.Db;

public partial class SharedFolder : BaseEntity
{
    public int FolderLinkId { get; set; }

    public int UserId { get; set; }

    public virtual FolderLink FolderLink { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
