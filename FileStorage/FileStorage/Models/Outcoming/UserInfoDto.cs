namespace FileStorage.Models.Outcoming
{
    public class UserInfoDto
    {
        public string? Username { get; set; }

        public string FirstName { get; set; } = null!;

        public string SecondName { get; set; } = null!;

        public string? About { get; set; }

        public string PrimaryEmail { get; set; } = null!;
    }
}
