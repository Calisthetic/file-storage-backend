namespace FileStorage.Models.Db;

public partial class File : BaseEntity
{
    public string Name { get; set; } = null!;

    public int? FolderId { get; set; }

    public int UserId { get; set; }

    public int FileTypeId { get; set; }

    public long FileSize { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<DownloadOfFile> DownloadsOfFiles { get; set; } = new List<DownloadOfFile>();

    public virtual ICollection<ElectedFile> ElectedFiles { get; set; } = new List<ElectedFile>();

    public virtual Folder? Folder { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual FileType FileType { get; set; } = null!;

    public virtual ICollection<ViewOfFile> ViewsOfFiles { get; set; } = new List<ViewOfFile>();
}
