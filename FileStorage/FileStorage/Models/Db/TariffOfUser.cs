namespace FileStorage.Models.Db;

public partial class TariffOfUser : BaseEntity
{
    public int TariffId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime EndAt { get; set; }

    public virtual Tariff Tariff { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
