namespace FileStorage.Models.Incoming
{
    public class FolderCreateDto
    {
        public string Name { get; set; } = null!;

        public int? UpperFolderId { get; set; }
    }
}
