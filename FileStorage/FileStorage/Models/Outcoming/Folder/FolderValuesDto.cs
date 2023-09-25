using FileStorage.Models.Outcoming.File;

namespace FileStorage.Models.Outcoming.Folder
{
    public class FolderValuesDto
    {
        public List<FolderInfoDto> Folders { get; set; } = new List<FolderInfoDto>();

        public List<FileInfoDto> Files { get; set; } = new List<FileInfoDto>();
    }
}
