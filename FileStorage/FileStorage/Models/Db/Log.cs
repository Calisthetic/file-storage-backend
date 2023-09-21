namespace FileStorage.Models.Db;

public partial class Log : BaseEntity
{
    public string Message { get; set; } = null!;

    public string? MessageTemplate { get; set; }

    public string? Level { get; set; }

    public DateTime? TimeStamp { get; set; }

    public string? Exception { get; set; }

    public string? Properties { get; set; }
}
