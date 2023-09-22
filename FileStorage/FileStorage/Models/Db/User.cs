namespace FileStorage.Models.Db;

public class User : BaseEntity
{
    public User()
    {
        Emails = new HashSet<Email>();
    }

    public string? Username { get; set; }

    public string FirstName { get; set; } = null!;

    public string SecondName { get; set; } = null!;

    public string? About { get; set; }

    public string Password { get; set; } = null!;

    public int? InvitedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? PrimaryEmailId { get; set; }

    public bool IsBlocked { get; set; }

    public virtual ICollection<Action> Actions { get; set; } = new List<Action>();

    public virtual ICollection<DownloadOfFile> DownloadsOfFiles { get; set; } = new List<DownloadOfFile>();

    public virtual ICollection<DownloadOfFolder> DownloadsOfFolders { get; set; } = new List<DownloadOfFolder>();

    public virtual ICollection<ElectedFile> ElectedFiles { get; set; } = new List<ElectedFile>();

    public virtual ICollection<ElectedFolder> ElectedFolders { get; set; } = new List<ElectedFolder>();

    public virtual ICollection<Email> Emails { get; set; } = new List<Email>();

    public virtual ICollection<File> Files { get; set; } = new List<File>();

    public virtual ICollection<Folder> Folders { get; set; } = new List<Folder>();

    public virtual Email? PrimaryEmail { get; set; }

    public virtual ICollection<TariffOfUser> TariffsOfUsers { get; set; } = new List<TariffOfUser>();

    public virtual ICollection<ViewOfFile> ViewsOfFiles { get; set; } = new List<ViewOfFile>();

    public virtual ICollection<ViewOfFolder> ViewsOfFolders { get; set; } = new List<ViewOfFolder>();
}
