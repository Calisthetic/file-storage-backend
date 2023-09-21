namespace FileStorage.Models.Db;

public partial class DownloadOfFolder : BaseEntity
{
    public int FolderId { get; set; }

    public int UserId { get; set; }

    public virtual Folder Folder { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
