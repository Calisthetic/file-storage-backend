namespace FileStorage.Models.Incoming
{
    public class UserSignUpDto
    {
        public string FirstName { get; set; } = null!;

        public string SecondName { get; set; } = null!;

        public string? About { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
