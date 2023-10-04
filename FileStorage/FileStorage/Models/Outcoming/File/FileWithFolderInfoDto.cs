namespace FileStorage.Models.Outcoming.File
{
    public class FileWithFolderInfoDto : FileInfoDto
    {
        public string FolderToken { get; set; } = null!;

        public string FolderName { get; set;} = null!;
    }
}
