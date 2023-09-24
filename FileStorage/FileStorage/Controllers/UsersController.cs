using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileStorage.Data;
using FileStorage.Models.Db;
using FileStorage.Models.Outcoming;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FileStorage.Models.Incoming.User;
using FileStorage.Services;

namespace FileStorage.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ApiDbContext _context;

        public UsersController(ApiDbContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return await _mapper.From(_context.Users.Include(x => x.PrimaryEmail)).ProjectToType<UserInfoDto>().ToListAsync();
        }
        // GET: api/users
        [HttpGet("test")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetUsersTest()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return await _mapper.From(_context.Users.Include(x => x.PrimaryEmail)).ProjectToType<UserInfoDto>().ToListAsync();
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPatch("profile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PathUserProfile(UserPatchProfileDto newData)
        {

            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            if (newData.Username != null)
                currentUser.Username = newData.Username;
            if (newData.PrimaryEmailId is not null)
                if (EmailExists((int)newData.PrimaryEmailId))
                    currentUser.PrimaryEmailId = newData.PrimaryEmailId;
            if (newData.About is not null)
                currentUser.About = newData.About;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("account")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PathUserBirthday(UserPatchAccountDto newData)
        {
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            if (!DateTime.TryParse(newData.Birthday, out DateTime result)) // 22/12/2011
            {
                return BadRequest("Wrong date type");
            }

            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            if (newData.Birthday != null)
                currentUser.Birthday = result;
            if (newData.FirstName != null)
                currentUser.FirstName = newData.FirstName;
            if (newData.SecondName != null)
                currentUser.SecondName = newData.SecondName;
            // work with emails !!!

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmailExists(int id)
        {
            return (_context.Emails?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
