namespace FileStorage.Models.Db;

public partial class Folder : BaseEntity
{
    public string Token { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int? UpperFolderId { get; set; }

    public int UserId { get; set; }

    public int? AccessTypeId { get; set; }

    public string? Color { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AccessType? AccessType { get; set; }

    public virtual ICollection<DownloadOfFolder> DownloadsOfFolders { get; set; } = new List<DownloadOfFolder>();

    public virtual ICollection<ElectedFolder> ElectedFolders { get; set; } = new List<ElectedFolder>();

    public virtual ICollection<File> Files { get; set; } = new List<File>();

    public virtual ICollection<Folder> InverseUpperFolder { get; set; } = new List<Folder>();

    public virtual Folder? UpperFolder { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<ViewOfFolder> ViewsOfFolders { get; set; } = new List<ViewOfFolder>();
}
