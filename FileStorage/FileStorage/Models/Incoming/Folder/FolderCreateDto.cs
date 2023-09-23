namespace FileStorage.Models.Incoming.Folder
{
    public class FolderCreateDto
    {
        public string Name { get; set; } = null!;

        public string? UpperFolderToken { get; set; }
    }
}
