namespace FileStorage.Models.Incoming.User
{
    public class UserPatchAccountDto
    {
        public string? FirstName { get; set; }

        public string? SecondName { get; set; }

        public string? Birthday { get; set; }

        public string[]? Emails { get; set; }
    }
}
