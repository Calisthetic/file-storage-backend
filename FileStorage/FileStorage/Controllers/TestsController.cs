using Asp.Versioning;
using FileStorage.Data;
using FileStorage.Models.Db;
using FileStorage.Models.Outcoming.File;
using FileStorage.Models.Outcoming.Folder;
using FileStorage.Services;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/tests")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public TestsController(ApiDbContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        // GET: api/Files
        [HttpGet("files")]
        public async Task<ActionResult<IEnumerable<Models.Db.File>>> GetFiles()
        {
            if (_context.Files == null)
            {
                return NotFound();
            }
            return await _context.Files.ToListAsync();
        }

        // GET: api/Files
        [HttpGet("folders")]
        public async Task<ActionResult<IEnumerable<FolderTreeDto>>> GetFolders()
        {
            if (_context.Files == null || _context.Folders == null)
            {
                return NotFound();
            }

            var files = await _mapper.From(
                _context.Files.Where(x => x.IsDeleted == false && x.FolderId == null)
            ).ProjectToType<FileTreeDto>().ToListAsync();
            var folders = await _mapper.From(
                _context.Folders.Include(x => x.InverseUpperFolder).Include(x => x.Files)
            ).ProjectToType<FolderInfoDto>().ToListAsync();
            if (folders == null) { return NotFound(); }

            return Ok(new { files = files, folders = folders });
        }

        // GET: api/Files
        [HttpGet("fileTypes")]
        public async Task<ActionResult<IEnumerable<FileType>>> GetFileTypes()
        {
            if (_context.FileTypes == null)
            {
                return NotFound();
            }
            return await _context.FileTypes.ToListAsync();
        }

        // GET: api/Files
        [HttpGet("be")]
        public async Task<IActionResult> GetFileBe()
        {
            if (_context.FileTypes == null)
            {
                return NotFound();
            }
            return Ok(await _context.ViewsOfFiles.Where(x => x.UserId == 1).OrderBy(x => x.FileId).ToListAsync());
        }
    }
}
