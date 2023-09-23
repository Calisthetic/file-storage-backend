namespace FileStorage.Models.Incoming.User
{
    public class UserPatchProfileDto
    {
        public string? Username { get; set; }

        public int? PrimaryEmailId { get; set; }

        public string? About { get; set; }
    }
}
