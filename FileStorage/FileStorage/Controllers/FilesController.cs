using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileStorage.Data;
using FileStorage.Models.Db;
using FileStorage.Models.Incoming.File;
using FileStorage.Services;
using System.Diagnostics;
using FileStorage.Models.Incoming.Folder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace FileStorage.Controllers
{
    [Route("api/file")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public FilesController(ApiDbContext context, IConfiguration configuration, IUserService userService)
        {
            _context = context;
            _configuration = configuration;
            _userService = userService;
        }

        // GET: api/file/download/5
        [HttpGet("download/{id}")]
        public async Task<ActionResult> GetFile(int id)
        {
            if (_context.Files == null)
            {
                return NotFound();
            }

            var file = await _context.Files.FindAsync(id);
            if (file == null)
            {
                return NotFound();
            }

            string path = _configuration.GetSection("StoragePath").Value!;
            string localFilePath = path + @"\" + (file.Folder?.UserId ?? file.UserId) + @"\" + file.Id + file.Name[file.Name.LastIndexOf('.')..];

            if (System.IO.File.Exists(localFilePath))
            {
                return File(System.IO.File.OpenRead(localFilePath), "application/octet-stream", file.Name);
            }

            return NotFound();
        }

        // POST: api/file
        [HttpPost]
        [ActionName(nameof(PostFile))]
        public async Task<IActionResult> PostFile([FromForm] FileCreateDto filesData)
        {
            if (_context.Files == null)
            {
                return Problem("Entity set 'ApiDbContext.Files' is null.");
            }

            // Configure path to save
            string path = _configuration.GetSection("StoragePath").Value!;
            Debug.WriteLine(filesData.FolderToken);
            Debug.WriteLine(filesData.Files.Count);

            // Check folder and files
            var currentFolder = await _context.Folders.FirstOrDefaultAsync(x => x.Token == filesData.FolderToken);
            if (((currentFolder == null || filesData.Files.Count == 0) && filesData.FolderToken != "main") || filesData.Files.Count == 0)
            {
                return BadRequest();
            }

            // Check auth user
            int? userId = null;
            if (int.TryParse(_userService.GetUserId(), out int userIdResult))
            {
                userId = userIdResult;
            }

            // (don't) Require auth and access check || owner check
            if ((currentFolder != null && (userId == currentFolder.UserId || (currentFolder.AccessType != null &&
                (currentFolder.AccessType.RequireAuth == false || userId != null) && currentFolder.AccessType.CanEdit == true))) || filesData.FolderToken == "main")
            {
                // Set path and create if not exists
                path = path + "\\" + userId.ToString();
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                long sizeSum = 0;
                foreach (var file in filesData.Files)
                {
                    Debug.WriteLine(file.FileName);
                    Debug.WriteLine(file.ContentType);
                    Debug.WriteLine(file.Length);
                    sizeSum += file.Length;
                }

                // Usage limits
                // !!! change later
                long currentSize = 0;
                FileInfo[] files = new DirectoryInfo(path).GetFiles();
                foreach (FileInfo file in files)
                {
                    currentSize += file.Length;
                }
                if (currentSize + sizeSum > 1000000000)
                {
                    return BadRequest("Usage limit");
                }

                foreach (var file in filesData.Files)
                {
                    // Add new if filetype don't exist
                    var currentFileType = await _context.FileTypes.FirstOrDefaultAsync(x => x.Name == file.ContentType);
                    if (currentFileType == null)
                    {
                        currentFileType = new FileType() { Name = file.ContentType };
                        _context.FileTypes.Add(currentFileType);
                        await _context.SaveChangesAsync();
                    }

                    // Add file to database
                    var newFile = new Models.Db.File()
                    {
                        Name = file.FileName,
                        FolderId = currentFolder?.Id,
                        UserId = userId ?? currentFolder.UserId,
                        FileTypeId = currentFileType.Id,
                        FileSize = file.Length,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now
                    };
                    _context.Add(newFile);
                    await _context.SaveChangesAsync();

                    // If file already exists
                    string newFilePath = path + "\\" + newFile.Id + file.FileName[file.FileName.LastIndexOf('.')..];
                    if (Path.Exists(newFilePath))
                    {
                        System.IO.File.Delete(newFilePath);
                    }
                    // Create file at storage
                    string filepath = Path.Combine(path, newFile.Id + file.FileName[file.FileName.LastIndexOf('.')..]);
                    using (Stream stream = new FileStream(filepath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                return CreatedAtAction(nameof(PostFile), filesData.Files.Count);
            }
            else
            {
                return BadRequest();
            }
        }
        
        // PATCH: api/file/bin/{id}
        [HttpPatch("bin/{id}")]
        public async Task<IActionResult> PatchFileBin(int id)
        {
            if (_context.Files == null)
            {
                return NotFound();
            }
            var currentFile = await _context.Files.Include(x => x.Folder).FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (currentFile == null)
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
            if (userId == currentFile.Folder?.UserId || (userId == currentFile.UserId && currentFile.FolderId == null) ||
                (currentFile.Folder != null && currentFile.Folder.AccessType != null &&
                (currentFile.Folder.AccessType.RequireAuth == false || userId != null) && currentFile.Folder.AccessType.CanEdit == true))
            {
                currentFile.IsDeleted = true;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        // PATCH: api/file/restore/{id}
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> PatchFileRestore(int id)
        {
            if (_context.Files == null)
            {
                return NotFound();
            }
            var currentFile = await _context.Files.Include(x => x.Folder).FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == true);
            if (currentFile == null)
            {
                return NotFound();
            }

            // If user authorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            // owner check
            if (userId == currentFile.UserId)
            {
                currentFile.IsDeleted = false;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Delete File
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/file/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            if (_context.Files == null)
            {
                return NotFound();
            }
            var currentFile = await _context.Files.Include(x => x.Folder).FirstOrDefaultAsync(x => x.Id == id);
            if (currentFile == null)
            {
                return NotFound();
            }

            // If user authorized
            int? userId = null;
            if (int.TryParse(_userService.GetUserId(), out int userIdResult))
            {
                userId = userIdResult;
            }

            // Configure path to delete
            string path = _configuration.GetSection("StoragePath").Value!;

            // (don't) Require auth and access check || owner check
            if (userId == currentFile.Folder?.UserId || (userId == currentFile.UserId && currentFile.FolderId == null) || 
                (currentFile.Folder != null && currentFile.Folder.AccessType != null &&
                (currentFile.Folder.AccessType.RequireAuth == false || userId != null) && currentFile.Folder.AccessType.CanEdit == true))
            {
                _context.Files.Remove(currentFile);
                await _context.SaveChangesAsync();

                if (System.IO.File.Exists(path + "\\" + userId + "\\" + currentFile.Id + currentFile.Name[currentFile.Name.LastIndexOf('.')..]))
                    System.IO.File.Delete(path + "\\" + userId + "\\" + currentFile.Id + currentFile.Name[currentFile.Name.LastIndexOf('.')..]);
                else
                    return NotFound();

                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        // PATCH: api/file/name/{id}
        [HttpPatch("name/{id}")]
        public async Task<IActionResult> PatchFileName(int id, FilePatchNameDto fileData)
        {
            Models.Db.File? currentFile = await _context.Files.Include(x => x.Folder).FirstOrDefaultAsync(x => x.Id == id);
            if (currentFile == null)
            {
                return NotFound();
            }

            // If user authorized
            int? userId = null;
            if (int.TryParse(_userService.GetUserId(), out int userIdResult))
            {
                userId = userIdResult;
            }

            // (don't) Require auth and access check || main folder check
            if ((userId == currentFile.UserId && currentFile.Folder == null) || (currentFile.Folder != null && currentFile.Folder.AccessType != null &&
                (currentFile.Folder.AccessType.RequireAuth == false || userId != null) && currentFile.Folder.AccessType.CanEdit == true))
            {
                currentFile.Name = fileData.Name;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        // PATCH: api/file/elect/{id}
        [HttpPatch("elect/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PatchFolderElect(int id)
        {
            // If current && destination folder exists
            Models.Db.File? currentFile = await _context.Files.Include(x => x.Folder).FirstOrDefaultAsync(x => x.Id == id);
            if (currentFile == null)
            {
                return NotFound();
            }

            // If user authorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            // owner check || access check
            if ((currentFile.FolderId == null && currentFile.UserId == userId) || (currentFile.Folder != null && (userId == currentFile.Folder.UserId || currentFile.Folder.AccessType != null)))
            {
                var currentElect = await _context.ElectedFolders.FirstOrDefaultAsync(x => x.UserId == userId && x.FolderId == currentFile.Id);
                if (currentElect == null)
                {
                    await _context.ElectedFiles.AddAsync(new ElectedFile() { FileId = currentFile.Id, UserId = userId });
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

        private bool FileExists(int id)
        {
            return (_context.Files?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
