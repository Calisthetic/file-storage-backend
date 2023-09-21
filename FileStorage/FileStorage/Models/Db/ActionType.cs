namespace FileStorage.Models.Db;

public partial class ActionType : BaseEntity
{
    public string Name { get; set; } = null!;

    public virtual ICollection<Action> Actions { get; set; } = new List<Action>();
}
