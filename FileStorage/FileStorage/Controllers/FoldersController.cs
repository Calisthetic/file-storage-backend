﻿using Microsoft.AspNetCore.Mvc;
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
using FileStorage.Models.Outcoming;
using Mapster;
using FileStorage.Models.Outcoming.File;

namespace FileStorage.Controllers
{
    [Route("api/folder")]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;

        public FoldersController(ApiDbContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        /// <summary>
        /// Files/Folder inside current folder
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        // GET: api/folder/{token}
        [HttpGet("{token}")]
        public async Task<ActionResult<FolderValuesDto>> GetFolder(string token)
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

            var currentElect = await _context.ElectedFolders.FirstOrDefaultAsync(x => x.UserId == userId && x.FolderId == currentFolder.Id);
            // !!! change later
            // owner check || access check
            if (userId == currentFolder.UserId || currentFolder.AccessType != null)
            {
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
            if (!string.IsNullOrEmpty(folderData.UpperFolderToken) && folderData.UpperFolderToken != "main")
            {
                Folder? upperFolder = await _context.Folders.FirstOrDefaultAsync(x => x.UpperFolder != null && x.UpperFolder.Token == folderData.UpperFolderToken);
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
                if (await _context.Folders.Where(x => x.Token == temp).CountAsync() == 0)
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
