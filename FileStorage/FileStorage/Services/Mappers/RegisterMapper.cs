using FileStorage.Models.Db;
using FileStorage.Models.Outcoming;
using FileStorage.Models.Outcoming.File;
using FileStorage.Models.Outcoming.Folder;
using Mapster;

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
                .Map(d => d.Views, r => r.ViewsOfFolders.Count)
                .Map(d => d.FilesInside, r => r.Files.Count)
                .Map(d => d.IsElected, r => r.ElectedFolders.Count != 0)
                .Map(d => d.AccessType, r => r.AccessType == null ? null : r.AccessType.Name)
                .Map(d => d.Size, r => r.IsDeleted ? 0 : 0)
                .Map(d => d.CreatedAt, r => r.CreatedAt.ToString().Substring(0,19).Replace('T',' '))
                .RequireDestinationMemberSource(true);

            // Files
            config.NewConfig<Models.Db.File, FileInfoDto>()
                .Map(d => d.Token, r => r.Id)
                .Map(d => d.Downloads, r => r.DownloadsOfFiles.Count)
                .Map(d => d.Views, r => r.ViewsOfFiles.Count)
                .Map(d => d.FileType, r => r.FileType.Name)
                .Map(d => d.IsElected, r => r.ElectedFiles.Count != 0)
                .Map(d => d.CreatedAt, r => r.CreatedAt.ToString().Substring(0, 19).Replace('T', ' '))
                .RequireDestinationMemberSource(true);
        }
    }
}
