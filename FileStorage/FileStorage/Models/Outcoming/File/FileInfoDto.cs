using FileStorage.Models.Db;

namespace FileStorage.Models.Outcoming.File
{
    public class FileInfoDto
    {
        public string Token = null!;

        public string Name { get; set; } = null!;

        public long FileSize { get; set; }

        public string CreatedAt { get; set; }

        public int? Downloads { get; set; }

        public int? Views { get; set; }

        public string FileType { get; set; } = null!;

        public bool IsElected { get; set; }
    }
}
