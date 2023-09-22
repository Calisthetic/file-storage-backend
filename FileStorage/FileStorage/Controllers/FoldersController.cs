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
    public class FoldersController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public FoldersController(ApiDbContext context)
        {
            _context = context;
        }

        // GET: api/Folders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Folder>>> GetFolders()
        {
          if (_context.Folders == null)
          {
              return NotFound();
          }
            return await _context.Folders.ToListAsync();
        }

        // GET: api/Folders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Folder>> GetFolder(int id)
        {
          if (_context.Folders == null)
          {
              return NotFound();
          }
            var folder = await _context.Folders.FindAsync(id);

            if (folder == null)
            {
                return NotFound();
            }

            return folder;
        }

        // PUT: api/Folders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFolder(int id, Folder folder)
        {
            if (id != folder.Id)
            {
                return BadRequest();
            }

            _context.Entry(folder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FolderExists(id))
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

        // POST: api/Folders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Folder>> PostFolder(Folder folder)
        {
          if (_context.Folders == null)
          {
              return Problem("Entity set 'ApiDbContext.Folders'  is null.");
          }
            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFolder", new { id = folder.Id }, folder);
        }

        // DELETE: api/Folders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFolder(int id)
        {
            if (_context.Folders == null)
            {
                return NotFound();
            }
            var folder = await _context.Folders.FindAsync(id);
            if (folder == null)
            {
                return NotFound();
            }

            _context.Folders.Remove(folder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FolderExists(int id)
        {
            return (_context.Folders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
