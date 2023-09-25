namespace FileStorage.Models.Outcoming.Folder
{
    public class FolderInfoDto
    {
        public string Token { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? AccessType { get; set; }

        public string? Color { get; set; }

        public string CreatedAt { get; set; }

        public int? Downloads { get; set; }

        public int? Views { get; set; }

        public int FilesInside { get; set; }

        public bool IsElected { get; set; }

        public int Size { get; set; }
    }
}
