using FileStorage.Models.Db;
using FileStorage.Models.Outcoming;
using FileStorage.Models.Outcoming.File;
using FileStorage.Models.Outcoming.Folder;
using FileStorage.Models.Outcoming.Statistic;
using Mapster;
using Microsoft.AspNetCore.SignalR;

namespace FileStorage.Services.Mappers
{
    public class RegisterMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Users
            config.NewConfig<User, UserInfoDto>()
                .Map(dto => dto.PrimaryEmail, res => res.PrimaryEmail == null ? "" : res.PrimaryEmail.Name)
                .RequireDestinationMemberSource(true);

            // Folder
            config.NewConfig<Folder, FolderInfoDto>()
                .Map(d => d.Downloads, r => r.DownloadsOfFolders.Count)
                .Map(d => d.Views, r => r.ViewsOfFolders.Count == 0 ? 0 : r.ViewsOfFolders.Count)
                .Map(d => d.FilesInside, r => r.Files.Count)
                .Map(d => d.IsElected, r => r.ElectedFolders.Count != 0)
                .Map(d => d.AccessType, r => r.AccessType == null ? null : r.AccessType.Name)
                .Map(d => d.Size, r => r.IsDeleted ? 0 : 0)
                .Map(d => d.CreatedAt, r => r.CreatedAt.ToString().Substring(0, 19).Replace('T', ' '))
                .RequireDestinationMemberSource(true);
            config.NewConfig<ViewOfFolder, FolderInfoDto>()
                .Map(d => d.Token, r => r.Folder.Token)
                .Map(d => d.Name, r => r.Folder.Name)
                .Map(d => d.Color, r => r.Folder.Color)
                .Map(d => d.Downloads, r => r.Folder.DownloadsOfFolders.Count)
                .Map(d => d.Views, r => r.Folder.ViewsOfFolders.Count == 0 ? 0 : r.Folder.ViewsOfFolders.Count - 1)
                .Map(d => d.FilesInside, r => r.Folder.Files.Count)
                .Map(d => d.IsElected, r => r.Folder.ElectedFolders.Count != 0)
                .Map(d => d.AccessType, r => r.Folder.AccessType == null ? null : r.Folder.AccessType.Name)
                .Map(d => d.Size, r => r.Folder.IsDeleted ? 0 : 0)
                .Map(d => d.CreatedAt, r => r.Folder.CreatedAt.ToString().Substring(0, 19).Replace('T', ' '))
                .RequireDestinationMemberSource(true);

            // Files
            config.NewConfig<Models.Db.File, FileInfoDto>()
                .Map(d => d.Token, r => r.Id)
                .Map(d => d.Downloads, r => r.DownloadsOfFiles.Count)
                .Map(d => d.Views, r => r.ViewsOfFiles.Count == 0 ? 0 : r.ViewsOfFiles.Count - 1)
                .Map(d => d.FileType, r => r.FileType.Name)
                .Map(d => d.IsElected, r => r.ElectedFiles.Count != 0)
                .Map(d => d.CreatedAt, r => r.CreatedAt.ToString().Substring(0, 19).Replace('T', ' '))
                .RequireDestinationMemberSource(true);
            config.NewConfig<Models.Db.File, FileWithFolderInfoDto>()
                .Map(d => d.FolderName, r => r.Folder == null ? "Main" : r.Folder.Name)
                .Map(d => d.FolderToken, r => r.Folder == null ? "main" : r.Folder.Token)
                .Map(d => d.Token, r => r.Id)
                .Map(d => d.Downloads, r => r.DownloadsOfFiles.Count)
                .Map(d => d.Views, r => r.ViewsOfFiles.Count == 0 ? 0 : r.ViewsOfFiles.Count - 1)
                .Map(d => d.FileType, r => r.FileType.Name)
                .Map(d => d.IsElected, r => r.ElectedFiles.Count != 0)
                .Map(d => d.CreatedAt, r => r.CreatedAt.ToString().Substring(0, 19).Replace('T', ' '))
                .RequireDestinationMemberSource(true);

            // Elected
            config.NewConfig<ElectedFolder, FolderElectedInfoDto>()
                .Map(d => d.IsDeleted, r => r.Folder.IsDeleted)
                .Map(d => d.Token, r => r.Folder.Token)
                .Map(d => d.Name, r => r.Folder.Name)
                .Map(d => d.Color, r => r.Folder.Color)
                .Map(d => d.Downloads, r => r.Folder.DownloadsOfFolders.Count)
                .Map(d => d.Views, r => r.Folder.ViewsOfFolders.Count == 0 ? 0 : r.Folder.ViewsOfFolders.Count - 1)
                .Map(d => d.FilesInside, r => r.Folder.Files.Count)
                .Map(d => d.IsElected, r => r.Folder.ElectedFolders.Count != 0)
                .Map(d => d.AccessType, r => r.Folder.AccessType == null ? null : r.Folder.AccessType.Name)
                .Map(d => d.Size, r => r.Folder.IsDeleted ? 0 : 0)
                .Map(d => d.CreatedAt, r => r.Folder.CreatedAt.ToString().Substring(0, 19).Replace('T', ' '))
                .RequireDestinationMemberSource(true);
            config.NewConfig<ElectedFile, FileElectedInfoDto>()
                .Map(d => d.FolderToken, r => r.File.Folder == null ? "main" : r.File.Folder.Token)
                .Map(d => d.Name, r => r.File.Name)
                .Map(d => d.FileSize, r => r.File.FileSize)
                .Map(d => d.Token, r => r.File.Id)
                .Map(d => d.Downloads, r => r.File.DownloadsOfFiles.Count)
                .Map(d => d.Views, r => r.File.ViewsOfFiles.Count == 0 ? 0 : r.File.ViewsOfFiles.Count - 1)
                .Map(d => d.FileType, r => r.File.FileType.Name)
                .Map(d => d.IsElected, r => r.File.ElectedFiles.Count != 0)
                .Map(d => d.CreatedAt, r => r.File.CreatedAt.ToString().Substring(0, 19).Replace('T', ' '))
                .RequireDestinationMemberSource(true);

            // Statistic
            config.NewConfig<Models.Db.File, FileTreeDto>()
                .Map(d => d.Name, r => r.Name)
                .RequireDestinationMemberSource(true);
            config.NewConfig<Folder, FolderTreeDto>()
                .Map(d => d.Name, r => r.Name)
                .Map(d => d.Files, r => r.Files.Adapt<List<FileTreeDto>>())
                .Map(d => d.Folders, r => r.InverseUpperFolder.Adapt<List<FolderTreeDto>>())
                .RequireDestinationMemberSource(true);
        }
    }
}
