using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileStorage.Feedback.Data;
using FileStorage.Feedback.Models.Db;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using MapsterMapper;
using FileStorage.Feedback.Services;
using FileStorage.Feedback.Models.Incoming;

namespace FileStorage.Feedback.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/bugs")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BugsController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public BugsController(ApiDbContext context, IUserService userService, IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
        }

        // GET: api/bugs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bug>>> GetBugs()
        {
            if (_context.Bugs == null)
            {
                return NotFound();
            }

            // If user authorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            return await _context.Bugs.Where(x => x.UserId == userId).ToListAsync();
        }

        // PATCH: api/bugs/5
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> PutBug(int id, BugPatchStatusDto data)
        {
            if (_context.Bugs == null)
            {
                return NotFound();
            }

            var currentBug = await _context.Bugs.FirstOrDefaultAsync(x => x.Id == id);
            if (currentBug == null)
            {
                return NotFound();
            }

            currentBug.BugStatusId = data.BugStatusId;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/bugs
        [HttpPost]
        [ActionName(nameof(PostBug))]
        public async Task<ActionResult<Bug>> PostBug(BugCreateDto bug)
        {
            if (_context.Bugs == null)
            {
                return Problem("Entity set 'ApiDbContext.Bugs' is null.");
            }

            // If user authorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            var newBug = new Bug()
            {
                AppPartId = bug.AppPartId,
                BugStatusId = 1,
                UserId = userId,
                Text = bug.Text,
            };
            _context.Bugs.Add(newBug);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostBug), new { id = newBug.Id });
        }

        // DELETE: api/bugs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBug(int id)
        {
            if (_context.Bugs == null)
            {
                return NotFound();
            }
            var bug = await _context.Bugs.FindAsync(id);
            if (bug == null)
            {
                return NotFound();
            }

            _context.Bugs.Remove(bug);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BugExists(int id)
        {
            return (_context.Bugs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
