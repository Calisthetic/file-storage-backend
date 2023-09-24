using FileStorage.Data;
using FileStorage.Models.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.Controllers
{
    [Route("api/tests")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public TestsController(ApiDbContext context)
        {
            _context = context;
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
        public async Task<ActionResult<IEnumerable<Folder>>> GetFolders()
        {
            if (_context.Folders == null)
            {
                return NotFound();
            }
            return await _context.Folders.ToListAsync();
        }
    }
}
