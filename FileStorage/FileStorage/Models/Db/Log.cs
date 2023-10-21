using System.ComponentModel.DataAnnotations.Schema;

namespace FileStorage.Models.Db;

public partial class Log : BaseEntity
{
    public string? Message { get; set; }

    public string? MessageTemplate { get; set; }

    public string? Level { get; set; }

    public DateTime? RaiseDate { get; set; }

    public string? Exception { get; set; }

    [Column(TypeName = "jsonb")]
    public string? Properties { get; set; }

    [Column(TypeName = "jsonb")]
    public string? PropsTest { get; set; }

    public string? MachineName { get; set; }
}
