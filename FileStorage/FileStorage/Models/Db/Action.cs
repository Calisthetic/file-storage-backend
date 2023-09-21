namespace FileStorage.Models.Db;

public partial class Action : BaseEntity
{
    public int UserId { get; set; }

    public int ActionTypeId { get; set; }

    public int Count { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ActionType ActionType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
