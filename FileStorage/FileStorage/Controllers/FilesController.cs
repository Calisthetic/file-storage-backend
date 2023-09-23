using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileStorage.Data;
using FileStorage.Models.Db;

namespace FileStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public FilesController(ApiDbContext context)
        {
            _context = context;
        }

        // GET: api/Files
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Db.File>>> GetFiles()
        {
          if (_context.Files == null)
          {
              return NotFound();
          }
            return await _context.Files.ToListAsync();
        }

        // GET: api/Files/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Db.File>> GetFile(int id)
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

            return file;
        }

        // PUT: api/Files/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFile(int id, Models.Db.File @file)
        {
            if (id != @file.Id)
            {
                return BadRequest();
            }

            _context.Entry(@file).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Files
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Models.Db.File>> PostFile(Models.Db.File @file)
        {
          if (_context.Files == null)
          {
              return Problem("Entity set 'ApiDbContext.Files'  is null.");
          }
            _context.Files.Add(@file);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FileExists(@file.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFile", new { id = @file.Id }, @file);
        }

        // DELETE: api/Files/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            if (_context.Files == null)
            {
                return NotFound();
            }
            var @file = await _context.Files.FindAsync(id);
            if (@file == null)
            {
                return NotFound();
            }

            _context.Files.Remove(@file);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FileExists(int id)
        {
            return (_context.Files?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
