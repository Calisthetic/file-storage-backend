using FileStorage.Data;
using FileStorage.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.Services;

public interface IStatisticService
{
    Task CalculateFileDownload(int userId, Models.Db.File file);
    Task CalculateFolderDownloads(int userId, Folder folder);
    Task CalculateViews(int? userId, Folder? folders = null, List<Models.Db.File>? files = null);
}

public class StatisticService : IStatisticService
{
    private readonly ApiDbContext _context;
    public StatisticService(ApiDbContext context)
    {
        _context = context;
    }



    public async Task CalculateFileDownload(int userId, Models.Db.File file)
    {
        var currentFile = await _context.DownloadsOfFiles.FirstOrDefaultAsync(x => x.UserId == userId && x.FileId == file.Id);
        if (currentFile == null)
        {
            await _context.AddAsync(new DownloadOfFile() { Id = userId, FileId = file.Id });
            await _context.SaveChangesAsync();
        }
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



    public async Task CalculateViews(int? userId, Folder? folder = null, List<Models.Db.File>? files = null)
    {
        if (userId == null) return;
        if (folder is not null)
        {
            ViewOfFolder? currentFolderView = await _context.ViewsOfFolders.FirstOrDefaultAsync(x => x.UserId == userId && x.FolderId == folder.Id);
            if (currentFolderView == null)
            {
                await _context.ViewsOfFolders.AddAsync(new ViewOfFolder() { UserId = (int)userId, FolderId = folder.Id, CreatedAt = DateTime.Now });
            }
            else if (currentFolderView != null)
            {
                currentFolderView.CreatedAt = DateTime.Now;
            }
            await _context.SaveChangesAsync();
        }
        if (files is not null)
        {
            for (int i = 0; i < files.Count; i++)
            {
                ViewOfFile? currentFileView = await _context.ViewsOfFiles.FirstOrDefaultAsync(x => x.UserId == userId && x.FileId == files[i].Id);
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
