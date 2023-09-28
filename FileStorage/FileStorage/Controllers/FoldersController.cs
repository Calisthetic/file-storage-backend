using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileStorage.Data;
using FileStorage.Models.Db;
using FileStorage.Services;
using MapsterMapper;
using FileStorage.Models.Incoming.Folder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using FileStorage.Models.Outcoming.Folder;
using Mapster;
using FileStorage.Models.Outcoming.File;
using System.IO.Compression;

namespace FileStorage.Controllers
{
    [Route("api/folder")]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ApiDbContext _context;
        private readonly IConfiguration _configuration;

        public FoldersController(ApiDbContext context, IMapper mapper, 
            IUserService userService, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _configuration = configuration;
        }

        // GET: api/folder/download/{token}
        [HttpGet("download/{token}")]
        public async Task<ActionResult> GetFolder(string token)
        {
            if (_context.Folders == null)
            {
                return NotFound();
            }

            // Search folder
            var currentFolder = await _context.Folders.Where(x => x.Token == token).Include(x => x.Files.Where(x => x.IsDeleted == false))
                .Include(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                    .ThenInclude(x => x.Files.Where(x => x.IsDeleted == false))
                .Include(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                    .ThenInclude(x => x.Files.Where(x => x.IsDeleted == false))
                .Include(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                    .ThenInclude(x => x.Files.Where(x => x.IsDeleted == false))
                .Include(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                    .ThenInclude(x => x.Files.Where(x => x.IsDeleted == false))
                .Include(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                    .ThenInclude(x => x.Files.Where(x => x.IsDeleted == false))
                .Include(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                    .ThenInclude(x => x.Files.Where(x => x.IsDeleted == false))
                .Include(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                    .ThenInclude(x => x.Files.Where(x => x.IsDeleted == false))
                .Include(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                .ThenInclude(x => x.InverseUpperFolder.Where(x => x.IsDeleted == false))
                    .ThenInclude(x => x.Files.Where(x => x.IsDeleted == false))
                    .FirstOrDefaultAsync();
            if (currentFolder == null)
            {
                return NotFound();
            }

            string path = _configuration.GetSection("StoragePath").Value!;
            string folderPath = path + @"\" + currentFolder.UserId;

            if (Directory.Exists(folderPath))
            {
                if (System.IO.File.Exists(folderPath + "\\" + "archive.zip"))
                    System.IO.File.Delete(folderPath + "\\" + "archive.zip");

                var fs = new FileStream(folderPath + "\\" + "archive.zip", FileMode.Create, FileAccess.ReadWrite);
                var zip = new ZipArchive(fs, ZipArchiveMode.Create);

                foreach (var file in currentFolder.Files)
                {
                    zip.CreateEntryFromFile(folderPath + "\\" + file.Id + file.Name[file.Name.LastIndexOf('.')..], file.Name);
                }
                foreach (var folder in currentFolder.InverseUpperFolder)
                {
                    Debug.WriteLine(folder.Name);
                    GetFolderData(folderPath, zip, folder);
                }

                zip.Dispose();
                fs.Close();

                return File(System.IO.File.OpenRead(folderPath + "\\" + "archive.zip"), "application/octet-stream", currentFolder.Name + ".zip");
            }
            return NotFound();
        }
        private static void GetFolderData(string path, ZipArchive zip, Folder folder)
        {
            string folderPath = GetFullPath(folder);
            zip.CreateEntry(folderPath);
            foreach (var file in folder.Files)
            {
                zip.CreateEntryFromFile(path + "\\" + file.Id + file.Name[file.Name.LastIndexOf('.')..], folderPath + file.Name);
            }

            foreach (var fold in folder.InverseUpperFolder)
            {
                GetFolderData(path, zip, fold);
            }
        }
        private static string GetFullPath(Folder folder, string path = "")
        {
            return folder.UpperFolder == null ? folder.Name + "/" + path : GetFullPath(folder.UpperFolder, folder.Name + "/" + path);
        }

        /// <summary>
        /// Files/Folder inside current folder
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        // GET: api/folder/{token}
        [HttpGet("{token}")]
        public async Task<ActionResult<FolderValuesDto>> GetFolderZip(string token)
        {
            if (_context.Folders == null)
            {
                return NotFound();
            }

            // If user authorized
            int? userId = null;
            if (int.TryParse(_userService.GetUserId(), out int userIdResult))
            {
                userId = userIdResult;
            }

            if (userId != null && token == "main")
            {
                var folders = await _mapper.From(
                    _context.Folders.Where(x => x.UserId == userId && x.UpperFolderId == null && x.IsDeleted == false)
                    .Include(x => x.Files)
                    .Include(x => x.DownloadsOfFolders)
                    .Include(x => x.ViewsOfFolders)
                    .Include(x => x.ElectedFolders.Where(x => x.UserId == userId))
                    .Include(x => x.AccessType)
                ).ProjectToType<FolderInfoDto>().ToListAsync();

                var files = await _mapper.From(
                    _context.Files.Where(x => x.UserId == userId && x.FolderId == null && x.IsDeleted == false)
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

            // Search folder
            var currentFolder = await _context.Folders.Include(x => x.AccessType).FirstOrDefaultAsync(x => x.Token == token);
            if (currentFolder == null)
            {
                return NotFound();
            }

            if (userId == currentFolder.UserId || (currentFolder.AccessType != null && currentFolder.AccessType.RequireAuth == false))
            {
                var folders = await _mapper.From(
                    _context.Folders.Where(x => x.UpperFolderId == currentFolder.Id && x.IsDeleted == false)
                    .Include(x => x.Files)
                    .Include(x => x.DownloadsOfFolders)
                    .Include(x => x.ViewsOfFolders)
                    .Include(x => x.ElectedFolders.Where(x => x.UserId == userId))
                    .Include(x => x.AccessType)
                ).ProjectToType<FolderInfoDto>().ToListAsync();

                var files = await _mapper.From(
                    _context.Files.Where(x => x.FolderId == currentFolder.Id && x.IsDeleted == false)
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
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Rename folder
        /// </summary>
        /// <param name="token"></param>
        /// <param name="folderData"></param>
        /// <returns></returns>
        // PATCH: api/folder/name/{token}
        [HttpPatch("name/{token}")]
        public async Task<IActionResult> PatchFolderName(string token, FolderPatchNameDto folderData)
        {
            Folder? currentFolder = await _context.Folders.FirstOrDefaultAsync(x => x.Token == token);
            if (currentFolder == null)
            {
                return NotFound();
            }

            // If user authorized
            int? userId = null;
            if (int.TryParse(_userService.GetUserId(), out int userIdResult))
            {
                userId = userIdResult;
            }

            // (don't) Require auth and access check || owner check
            if (userId == currentFolder.UserId || (currentFolder.AccessType != null &&
                (currentFolder.AccessType.RequireAuth == false || userId != null) && currentFolder.AccessType.CanEdit == true))
            {
                currentFolder.Name = folderData.Name;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Change color
        /// </summary>
        /// <param name="token"></param>
        /// <param name="folderData"></param>
        /// <returns></returns>
        // PATCH: api/folder/color/{token}
        [HttpPatch("color/{token}")]
        public async Task<IActionResult> PatchFolderName(string token, FolderPatchColorDto folderData)
        {
            Folder? currentFolder = await _context.Folders.FirstOrDefaultAsync(x => x.Token == token);
            if (currentFolder == null)
            {
                return NotFound();
            }

            // If user authorized
            int? userId = null;
            if (int.TryParse(_userService.GetUserId(), out int userIdResult))
            {
                userId = userIdResult;
            }

            // (don't) Require auth and access check || owner check
            if (userId == currentFolder.UserId || (currentFolder.AccessType != null &&
                (currentFolder.AccessType.RequireAuth == false || userId != null) && currentFolder.AccessType.CanEdit == true))
            {
                currentFolder.Color = folderData.Color;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Move folder
        /// </summary>
        /// <param name="token"></param>
        /// <param name="folderData"></param>
        /// <returns></returns>
        // PATCH: api/folder/path/{token}
        [HttpPatch("path/{token}")]
        public async Task<IActionResult> PatchFolderPath(string token, FolderPatchPathDto folderData)
        {
            // If current && destination folder exists
            Folder? currentFolder = await _context.Folders.FirstOrDefaultAsync(x => x.Token == token);
            Folder? destinationFolder = await _context.Folders.FirstOrDefaultAsync(x => x.Token == folderData.ToFolderToken);
            if (currentFolder == null || destinationFolder == null)
            {
                return NotFound();
            }

            // If user authorized
            int? userId = null;
            if (int.TryParse(_userService.GetUserId(), out int userIdResult))
            {
                userId = userIdResult;
            }

            // !!! change later
            // (don't) Require auth and access check || owner check
            if (userId == currentFolder.UserId || (currentFolder.AccessType != null &&
                (currentFolder.AccessType.RequireAuth == false || userId != null) && currentFolder.AccessType.CanEdit == true))
            {
                currentFolder.UpperFolderId = destinationFolder.Id;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Elect folder
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        // PATCH: api/folder/elect/{token}
        [HttpPatch("elect/{token}")]
        public async Task<IActionResult> PatchFolderElect(string token)
        {
            // If current && destination folder exists
            Folder? currentFolder = await _context.Folders.FirstOrDefaultAsync(x => x.Token == token);
            if (currentFolder == null)
            {
                return NotFound();
            }

            // If user authorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            // owner check || access check
            if ((userId == currentFolder.UserId && currentFolder.UpperFolderId == null) || currentFolder.AccessType != null)
            {
                var currentElect = await _context.ElectedFolders.FirstOrDefaultAsync(x => x.UserId == userId && x.FolderId == currentFolder.Id);
                if (currentElect == null)
                {
                    await _context.ElectedFolders.AddAsync(new ElectedFolder() { FolderId = currentFolder.Id, UserId = userId });
                }
                else
                {
                    _context.ElectedFolders.Remove(currentElect);
                }
                await _context.SaveChangesAsync();
                return NoContent();
            }
            return BadRequest();
        }

        /// <summary>
        /// Change access type of Folder
        /// </summary>
        /// <param name="token"></param>
        /// <param name="folderData"></param>
        /// <returns></returns>
        // PATCH: api/folder/access/{token}
        [HttpPatch("access/{token}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PatchFolderAccess(string token, FolderPatchAccessDto folderData)
        {
            Folder? currentFolder = await _context.Folders.FirstOrDefaultAsync(x => x.Token == token);
            if (currentFolder == null)
            {
                return NotFound();
            }

            // If user authorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            // (don't) Require auth and access check || owner check
            if (userId == currentFolder.UserId)
            {
                if (AccessTypeExists(folderData.AccessTypeId))
                {
                    currentFolder.AccessTypeId = folderData.AccessTypeId;
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                return BadRequest();
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Create Folder
        /// </summary>
        /// <param name="folderData"></param>
        /// <returns></returns>
        // POST: api/folder
        [HttpPost]
        [ActionName(nameof(PostFolder))]
        [AllowAnonymous]
        public async Task<ActionResult<Folder>> PostFolder(FolderCreateDto folderData)
        {
            if (_context.Folders == null)
            {
                return Problem("Entity set 'ApiDbContext.Folders' is null.");
            }

            // If user authorized
            int? userId = null;
            if (int.TryParse(_userService.GetUserId(), out int userIdResult))
            {
                userId = userIdResult;
            }

            // Upper folder check
            if (folderData.UpperFolderToken == "main" && userId != null)
            {
                Folder newFolder = await CreateFolder(folderData.Name, (int)userId, null);
                return CreatedAtAction(nameof(PostFolder), new { id = newFolder.Id });
            }
            else if (!string.IsNullOrEmpty(folderData.UpperFolderToken))
            {
                Folder? upperFolder = await _context.Folders.FirstOrDefaultAsync(x => x.Token == folderData.UpperFolderToken);
                if (upperFolder == null)
                {
                    return BadRequest("Upper folder doesn't exits");
                }
                // (don't) Require auth and access check || owner check
                else if (upperFolder.UserId == userId || (upperFolder.AccessType != null && 
                    (upperFolder.AccessType.RequireAuth == false || userId != null) && upperFolder.AccessType.CanEdit == true))
                {
                    Folder newFolder = await CreateFolder(folderData.Name, upperFolder.UserId, upperFolder.Id);
                    return CreatedAtAction(nameof(PostFolder), new { id = newFolder.Id });
                }
                else
                {
                    return BadRequest("You're not an owner or haven't acces to create folders");
                }
            }

            if (userId != null)
            {
                Folder newFolder = await CreateFolder(folderData.Name, (int)userId, null);
                return CreatedAtAction(nameof(PostFolder), new { id = newFolder.Id });
            }
            return Unauthorized();
        }
        private async Task<Folder> CreateFolder(string name, int userId, int? upperFolderId)
        {
            string token = string.Empty;
            while (string.IsNullOrEmpty(token))
            {
                string temp = "8" + RandomStringGeneration(31);
                if (!await _context.Folders.Where(x => x.Token == temp).AnyAsync())
                {
                    token = temp;
                }
            }
            Folder newFolder = new Folder() {
                Name = name,
                UpperFolderId = upperFolderId,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                UserId = userId,
                Token = token
            };
            await _context.Folders.AddAsync(newFolder);
            await _context.SaveChangesAsync();
            return newFolder;
        }

        /// <summary>
        /// Delete Folder
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        // DELETE: api/folder/{token}
        [HttpDelete("{token}")]
        public async Task<IActionResult> DeleteFolder(string token)
        {
            if (_context.Folders == null)
            {
                return NotFound();
            }
            var currentFolder = await _context.Folders.FirstOrDefaultAsync(x => x.Token == token);
            if (currentFolder == null)
            {
                return NotFound();
            }
            
            // If user authorized
            int? userId = null;
            if (int.TryParse(_userService.GetUserId(), out int userIdResult))
            {
                userId = userIdResult;
            }

            // (don't) Require auth and access check || owner check
            if (userId == currentFolder.UserId || (currentFolder.AccessType != null &&
                (currentFolder.AccessType.RequireAuth == false || userId != null) && currentFolder.AccessType.CanEdit == true))
            {
                _context.Folders.Remove(currentFolder);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        private bool FolderExists(int id)
        {
            return (_context.Folders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        private bool AccessTypeExists(int id)
        {
            return (_context.AccessTypes?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private string RandomStringGeneration(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPRSTUVWXYZ1234567890abcdefghijklmnoprstuvwxyz_";

            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
