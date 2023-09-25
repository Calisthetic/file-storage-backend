namespace FileStorage.Models.Db;

public partial class Tariff : BaseEntity
{
    public int Size { get; set; }

    public string UploadLimitName { get; set; } = null!;

    public bool UploadLimit { get; set; }

    public bool Customizable { get; set; }

    public bool ShowAd { get; set; }

    public bool IntegrationHelp { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<TariffOfUser> TariffsOfUsers { get; set; } = new List<TariffOfUser>();
}
