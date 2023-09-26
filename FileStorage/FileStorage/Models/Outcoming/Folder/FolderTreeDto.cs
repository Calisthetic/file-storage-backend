using FileStorage.Models.Outcoming.File;
using Mapster;

namespace FileStorage.Models.Outcoming.Folder
{
    public class FolderTreeDto
    {
        public string Name { get; set; } = null!;

        public List<FileTreeDto> Files { get; set; } = new List<FileTreeDto>();

        public List<FolderTreeDto> Folders { get; set; } = new List<FolderTreeDto>();
    }
}
