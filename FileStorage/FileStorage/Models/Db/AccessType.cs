namespace FileStorage.Models.Db;

public partial class AccessType : BaseEntity
{
    public string? Name { get; set; }

    public bool CanDownload { get; set; }

    public bool CanEdit { get; set; }

    public bool RequireAuth { get; set; }

    public virtual ICollection<FolderLink> FolderLinks { get; set; } = new List<FolderLink>();

    public virtual ICollection<Folder> Folders { get; set; } = new List<Folder>();
}
