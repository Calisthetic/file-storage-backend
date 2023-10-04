using FileStorage.Data;
using FileStorage.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.Services;

public interface IStatisticService
{
    Task CalculateViews(int? userId, List<Folder> folders, List<Models.Db.File> files);
}

public class StatisticService : IStatisticService
{
    private readonly ApiDbContext _context;
    public StatisticService(ApiDbContext context)
    {
        _context = context;
    }



    public async Task CalculateFolderDownloads(int userId, Folder folder)
    {
        await AddFolderDownloads(userId, folder);
        await _context.SaveChangesAsync();
    }
    private async Task AddFolderDownloads(int userId, Folder folder)
    {
        foreach (var file in folder.Files)
        {
            DownloadOfFile? currentFileDownload = await _context.DownloadsOfFiles.FirstOrDefaultAsync(x => x.UserId == userId && x.FileId == file.Id);
            if (currentFileDownload is null)
            {
                await _context.DownloadsOfFiles.AddAsync(new DownloadOfFile() { FileId = file.Id, UserId = userId });
            }
        }
        foreach (var fold in folder.InverseUpperFolder)
        {
            DownloadOfFolder? currentFolderDownload = await _context.DownloadsOfFolders.FirstOrDefaultAsync(x => x.UserId == userId && x.FolderId == fold.Id);
            if (currentFolderDownload is null)
            {
                await _context.DownloadsOfFolders.AddAsync(new DownloadOfFolder() { UserId = userId, FolderId = fold.Id });
            }
            await AddFolderDownloads(userId, fold);
        }
    }

    public async Task CalculateViews(int? userId, List<Folder> folders, List<Models.Db.File> files)
    {
        if (userId == null) return;
        if (folders is not null)
        {
            for (int i = 0; i < folders.Count; i++)
            {
                ViewOfFolder? currentFolderView = await _context.ViewsOfFolders.FirstOrDefaultAsync(x => x.UserId == userId);
                if (currentFolderView == null)
                {
                    await _context.ViewsOfFolders.AddAsync(new ViewOfFolder() { UserId = (int)userId, FolderId = folders[i].Id, CreatedAt = DateTime.Now });
                }
                else if (currentFolderView != null)
                {
                    currentFolderView.CreatedAt = DateTime.Now;
                }
            }
            await _context.SaveChangesAsync();
        }
        if (files is not null)
        {
            for (int i = 0; i < files.Count; i++)
            {
                ViewOfFile? currentFileView = await _context.ViewsOfFiles.FirstOrDefaultAsync(x => x.UserId == userId);
                if (currentFileView == null)
                {
                    await _context.ViewsOfFiles.AddAsync(new ViewOfFile() { UserId = (int)userId, FileId = files[i].Id, CreatedAt = DateTime.Now });
                }
                else if (currentFileView != null)
                {
                    currentFileView.CreatedAt = DateTime.Now;
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
