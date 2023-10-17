namespace FileStorage.Feedback.Models.Db;

public partial class BugStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Bug> Bugs { get; set; } = new List<Bug>();
}
