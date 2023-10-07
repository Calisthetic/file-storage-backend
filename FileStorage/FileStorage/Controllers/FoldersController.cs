using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileStorage.Data;
using FileStorage.Models.Db;
using FileStorage.Services;
using MapsterMapper;
using FileStorage.Models.Incoming.Folder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using FileStorage.Models.Outcoming.Folder;
using Mapster;
using FileStorage.Models.Outcoming.File;
using System.IO.Compression;
using System.Net;
using NuGet.Common;

namespace FileStorage.Controllers
{
    [Route("api/folders")]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ApiDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IStatisticService _statisticService;

        public FoldersController(ApiDbContext context, IMapper mapper, 
            IUserService userService, IConfiguration configuration, IStatisticService statisticService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _configuration = configuration;
            _statisticService = statisticService;
        }

        // GET: api/folders/download/{token}
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
            
            // If user authorized
            int? userId = null;
            if (int.TryParse(_userService.GetUserId(), out int userIdResult))
            {
                userId = userIdResult;
            }

            if (currentFolder.UserId == userId ||
                (currentFolder.AccessType != null &&
                (currentFolder.AccessType.RequireAuth == false || userId != null) && currentFolder.AccessType.CanDownload == true))
            {
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
                        GetFolderData(folderPath, zip, folder);
                    }

                    zip.Dispose();
                    fs.Close();

                    return File(System.IO.File.OpenRead(folderPath + "\\" + "archive.zip"), "application/octet-stream", currentFolder.Name + ".zip");
                }
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
            return folder.UpperFolder == null ? path : GetFullPath(folder.UpperFolder, folder.Name + "/" + path);
        }

        /// <summary>
        /// Files/Folder inside current folder
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        // GET: api/folders/{token}
        [HttpGet("{token}")]
        public async Task<ActionResult<FolderValuesDto>> GetFolderData(string token)
        {
            if (_context.Folders == null || _context.Files == null)
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
                var folders = await _context.Folders.Where(x => x.UserId == userId && x.UpperFolderId == null && x.IsDeleted == false)
                    .Include(x => x.Files)
                    .Include(x => x.DownloadsOfFolders)
                    .Include(x => x.ViewsOfFolders.Where(x => x.UserId != userId))
                    .Include(x => x.ElectedFolders.Where(x => x.UserId == userId))
                    .Include(x => x.AccessType).ToListAsync();

                var files = await _context.Files.Where(x => x.UserId == userId && x.FolderId == null && x.IsDeleted == false)
                    .Include(x => x.DownloadsOfFiles)
                    .Include(x => x.ViewsOfFiles.Where(x => x.UserId != userId))
                    .Include(x => x.ElectedFiles.Where(x => x.UserId == userId))
                    .Include(x => x.FileType).ToListAsync();

                await _statisticService.CalculateViews(userId, files: files);

                return Ok(new FolderValuesDto()
                {
                    Folders = folders.Adapt<List<FolderInfoDto>>(),
                    Files = files.Adapt<List<FileInfoDto>>(),
                });
            }

            // Search folder
            var currentFolder = await _context.Folders.Include(x => x.AccessType).FirstOrDefaultAsync(x => x.Token == token && x.IsDeleted == false);
            if (currentFolder == null)
            {
                return NotFound();
            }

            if (userId == currentFolder.UserId || (currentFolder.AccessType != null && (currentFolder.AccessType.RequireAuth == false || userId != null)))
            {
                var folders = await _context.Folders.Where(x => x.UpperFolderId == currentFolder.Id && x.IsDeleted == false)
                    .Include(x => x.Files)
                    .Include(x => x.DownloadsOfFolders)
                    .Include(x => x.ViewsOfFolders.Where(x => x.UserId != userId))
                    .Include(x => x.ElectedFolders.Where(x => x.UserId == userId))
                    .Include(x => x.AccessType).ToListAsync();

                var files = await _context.Files.Where(x => x.FolderId == currentFolder.Id && x.IsDeleted == false)
                    .Include(x => x.DownloadsOfFiles)
                    .Include(x => x.ViewsOfFiles.Where(x => x.UserId != userId))
                    .Include(x => x.ElectedFiles.Where(x => x.UserId == userId))
                    .Include(x => x.FileType).ToListAsync();

                await _statisticService.CalculateViews(userId, currentFolder, files);

                return Ok(new
                {
                    Folders = folders.Adapt<List<FolderInfoDto>>(),
                    Files = files.Adapt<List<FileInfoDto>>(),
                });
            }
            else
            {
                return NotFound();
            }
        }

        // GET: api/folders/elected
        [HttpGet("elected")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetFolderElected()
        {
            if (_context.Folders == null || _context.Files == null)
            {
                return NotFound();
            }

            // If user authorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            var folders = await _mapper.From(
                _context.ElectedFolders
                .Include(x => x.Folder)
                    .ThenInclude(x => x.Files)
                .Include(x => x.Folder)
                    .ThenInclude(x => x.DownloadsOfFolders)
                .Include(x => x.Folder)
                    .ThenInclude(x => x.ViewsOfFolders.Where(x => x.UserId != userId))
                .Where(x => x.UserId == userId)
            ).ProjectToType<FolderElectedInfoDto>().ToListAsync();

            var files = await _mapper.From(
                _context.ElectedFiles
                .Include(x => x.File)
                    .ThenInclude(x => x.DownloadsOfFiles)
                .Include(x => x.File)
                    .ThenInclude(x => x.ViewsOfFiles.Where(x => x.UserId != userId))
                .Include(x => x.File)
                    .ThenInclude(x => x.FileType)
                .Where(x => x.UserId == userId)
            ).ProjectToType<FileElectedInfoDto>().ToListAsync();

            return Ok(new
            {
                Folders = folders,
                Files = files,
            });
        }

        // GET: api/folders/recent
        [HttpGet("recent")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetFolderrecent()
        {
            if (_context.Folders == null || _context.Files == null)
            {
                return NotFound();
            }

            // If user authorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            var folders = await _mapper.From(
                _context.ViewsOfFolders
                .Include(x => x.Folder)
                    .ThenInclude(x => x.Files)
                .Include(x => x.Folder)
                    .ThenInclude(x => x.DownloadsOfFolders)
                .Include(x => x.Folder)
                    .ThenInclude(x => x.ViewsOfFolders.Where(x => x.UserId != userId))
                .OrderByDescending(x => x.CreatedAt)
                .Where(x => x.UserId == userId && x.CreatedAt != null)
            ).ProjectToType<FolderInfoDto>().ToListAsync();

            return Ok(folders);
        }

        // GET: api/folders/path/{token}
        [HttpGet("path/{token}")]
        public async Task<ActionResult<List<FolderSinglePath>>> GetFolderPath(string token)
        {
            // If current && destination folder exists
            Folder? currentFolder = await _context.Folders.Where(x => x.Token == token)
                .Include(x => x.UpperFolder)
                .ThenInclude(x => x.UpperFolder)
                .ThenInclude(x => x.UpperFolder)
                .ThenInclude(x => x.UpperFolder)
                .ThenInclude(x => x.UpperFolder)
                .FirstOrDefaultAsync();

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

            if (userId == currentFolder.UserId)
            {
                var result = GetPaths(currentFolder);
                result.Reverse();
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }
        }
        private static List<FolderSinglePath> GetPaths(Folder folder, List<FolderSinglePath>? paths = null)
        {
            paths ??= new List<FolderSinglePath>();

            paths.Add(new FolderSinglePath() { Name = folder.Name, Token = folder.Token });
            if (folder.UpperFolder != null)
            {
                paths = GetPaths(folder.UpperFolder, paths);
            }
            return paths;
        }

        // GET: api/folders/binName/{token}
        [HttpGet("binName/{token}")]
        public async Task<ActionResult> GetFolderBinName(string token)
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

            // Search folder
            var currentFolder = await _context.Folders.Include(x => x.AccessType).FirstOrDefaultAsync(x => x.Token == token);
            if (currentFolder == null)
            {
                return NotFound();
            }
            if (currentFolder.IsDeleted == false)
            {
                return Forbid();
            }

            if (userId == currentFolder.UserId || (currentFolder.AccessType != null && (currentFolder.AccessType.RequireAuth == false || userId != null)))
            {
                return Ok(new { Name = currentFolder.Name });
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
        // PATCH: api/folders/name/{token}
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
        // PATCH: api/folders/color/{token}
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
        // PATCH: api/folders/path/{token}
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
        // PATCH: api/folders/elect/{token}
        [HttpPatch("elect/{token}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PatchFolderElect(string token)
        {
            // If current folder exists
            Folder? currentFolder = await _context.Folders.Include(x => x.AccessType).FirstOrDefaultAsync(x => x.Token == token);
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
            if (userId == currentFolder.UserId || currentFolder.UpperFolderId == null || currentFolder.AccessType != null)
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
        /// UnElect folders && files
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        // PATCH: api/folders/elect/all
        [HttpPatch("elect/all")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PatchAllFoldersElect()
        {
            // If user authorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            var currentFolders = await _context.ElectedFolders.Where(x => x.UserId == userId).ToListAsync();
            var currentFiles = await _context.ElectedFiles.Where(x => x.UserId == userId).ToListAsync();

            _context .ElectedFolders.RemoveRange(currentFolders);
            _context.ElectedFiles.RemoveRange(currentFiles);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Change access type of Folder
        /// </summary>
        /// <param name="token"></param>
        /// <param name="folderData"></param>
        /// <returns></returns>
        // PATCH: api/folders/access/{token}
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

        // PATCH: api/folders/view/{token}
        [HttpPatch("view/{token}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PatchFolderView(string token)
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

            ViewOfFolder? currentView = await _context.ViewsOfFolders.FirstOrDefaultAsync(x => x.UserId == userId && x.FolderId == currentFolder.Id);
            if (currentView == null)
            {
                return BadRequest();
            }
            else
            {
                currentView.CreatedAt = null;
                await _context.SaveChangesAsync();
                return NoContent();
            }
        }

        /// <summary>
        /// Create Folder
        /// </summary>
        /// <param name="folderData"></param>
        /// <returns></returns>
        // POST: api/folders
        [HttpPost]
        [ActionName(nameof(PostFolder))]
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

        // PATCH: api/folders/bin/{token}
        [HttpPatch("bin/{token}")]
        public async Task<IActionResult> PatchFolderBin(string token)
        {
            if (_context.Folders == null)
            {
                return NotFound();
            }
            var currentFolder = await _context.Folders.FirstOrDefaultAsync(x => x.Token == token && x.IsDeleted == false);
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
                currentFolder.IsDeleted = true;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        // PATCH: api/folders/restore/{token}
        [HttpPatch("restore/{token}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PatchFolderRestore(string token)
        {
            if (_context.Folders == null)
            {
                return NotFound();
            }
            var currentFolder = await _context.Folders.FirstOrDefaultAsync(x => x.Token == token && x.IsDeleted == true);
            if (currentFolder == null)
            {
                return NotFound();
            }

            // If user authorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return NotFound();
            }

            // owner check
            if (userId == currentFolder.UserId)
            {
                currentFolder.IsDeleted = false;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Delete Folder
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        // DELETE: api/folders/{token}
        [HttpDelete("{token}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
                _context.Remove(currentFolder);
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

        private static string RandomStringGeneration(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPRSTUVWXYZ1234567890abcdefghijklmnoprstuvwxyz_";

            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
