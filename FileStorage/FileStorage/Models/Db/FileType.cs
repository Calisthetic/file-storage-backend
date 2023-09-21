namespace FileStorage.Models.Db;

public class FileType : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<File> Files { get; set; } = new List<File>();
}
