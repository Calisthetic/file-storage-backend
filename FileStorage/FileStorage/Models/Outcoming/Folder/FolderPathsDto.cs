namespace FileStorage.Models.Outcoming.Folder
{
    public class FolderPathsDto
    {
        public List<FolderSinglePath> Paths { get; set; } = new List<FolderSinglePath>();
    }

    public class FolderSinglePath
    {
        public string Name { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
