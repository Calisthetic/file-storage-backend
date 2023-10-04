namespace FileStorage.Models.Db;

public partial class ViewOfFolder : BaseEntity
{
    public int FolderId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Folder Folder { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
