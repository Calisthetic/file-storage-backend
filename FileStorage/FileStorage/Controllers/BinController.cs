using FileStorage.Data;
using FileStorage.Models.Db;
using FileStorage.Models.Outcoming.File;
using FileStorage.Models.Outcoming.Folder;
using FileStorage.Services;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.IO;

namespace FileStorage.Controllers
{
    [Route("api/bin")]
    [ApiController]
    public class BinController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public BinController(ApiDbContext context, IConfiguration configuration, IUserService userService, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _userService = userService;
            _mapper = mapper;
        }

        // GET: api/bin
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetBin()
        {
            // If user authorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            var folders = await _mapper.From(
                _context.Folders.Where(x => x.UserId == userId && x.IsDeleted == true)
                .Include(x => x.Files)
                .Include(x => x.DownloadsOfFolders)
                .Include(x => x.ViewsOfFolders)
                .Include(x => x.ElectedFolders.Where(x => x.UserId == userId))
                .Include(x => x.AccessType)
            ).ProjectToType<FolderInfoDto>().ToListAsync();

            var files = await _mapper.From(
                _context.Files.Where(x => x.UserId == userId && x.IsDeleted == true)
                .Include(x => x.DownloadsOfFiles)
                .Include(x => x.ViewsOfFiles)
                .Include(x => x.ElectedFiles.Where(x => x.UserId == userId))
                .Include(x => x.FileType)
            ).ProjectToType<FileInfoDto>().ToListAsync();

            return Ok(new FolderValuesDto()
            {
                Folders = folders,
                Files = files,
            });
        }

        // DELETE: api/bin
        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteBin()
        {
            // If user authorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            var folders = await _context.Folders.Where(x => x.UserId == userId && x.IsDeleted == true)
                .Include(x => x.Files)
                .Include(x => x.DownloadsOfFolders)
                .Include(x => x.ViewsOfFolders)
                .Include(x => x.ElectedFolders.Where(x => x.UserId == userId))
                .Include(x => x.AccessType).ToListAsync();

            var files = await _context.Files.Where(x => x.UserId == userId && x.IsDeleted == true)
                .Include(x => x.DownloadsOfFiles)
                .Include(x => x.ViewsOfFiles)
                .Include(x => x.ElectedFiles.Where(x => x.UserId == userId))
                .Include(x => x.FileType).ToListAsync();

            foreach (var folder in folders)
            {
                _context.Folders.Remove(folder);
                await _context.SaveChangesAsync();
            }

            // Configure path to delete
            string path = _configuration.GetSection("StoragePath").Value!;
            foreach (var file in files)
            {
                _context.Files.Remove(file);
                await _context.SaveChangesAsync();

                if (System.IO.File.Exists(path + "\\" + userId + "\\" + file.Id + "\\" + file.Name[file.Name.LastIndexOf('.')..]))
                    System.IO.File.Delete(path + "\\" + userId + "\\" + file.Id + "\\" + file.Name[file.Name.LastIndexOf('.')..]);
                else
                    return NotFound();
            }
            return NoContent();
        }
    }
}
