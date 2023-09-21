namespace FileStorage.Models.Db;

public partial class FolderLink : BaseEntity
{
    public int FolderId { get; set; }

    public string Token { get; set; } = null!;

    public int AccessTypeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? EndAt { get; set; }

    public virtual AccessType AccessType { get; set; } = null!;
}
