namespace FileStorage.Models.Incoming.File
{
    public class FileCreateDto
    {
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();

        public string FolderToken { get; set; } = null!;
    }
}
