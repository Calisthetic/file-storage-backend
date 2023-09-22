using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileStorage.Data;
using FileStorage.Models.Db;
using FileStorage.Models.Outcoming;
//using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
        private readonly IConfiguration _configuration;
        private readonly ApiDbContext _context;

        public UsersController(ApiDbContext context, IMapper mapper,
            IConfiguration configuration, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
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

        // PUT: api/users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        [HttpPatch("username")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PathUserUsername(UserPatchUsernameDto newName)
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

            currentUser.Username = newName.Username;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("birthday")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PathUserBirthday(UserPatchBirthdayDto newBirthday)
        {
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            if (!DateTime.TryParse(newBirthday.Birthday, out DateTime result)) // 22/12/2011
            {
                return BadRequest("Wrong data type");
            }

            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            currentUser.Birthday = result;
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

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }





        // POST: api/users/signin
        [AllowAnonymous]
        [HttpPost("signin")]
        public async Task<ActionResult<UserAuthResultDto>> SigninUser(UserSignInDto user)
        {
            if (_context.Users == null)
                return new JsonResult("Something went wrong") { StatusCode = 500 };

            // Search by (email || username) && password
            var existUser = await _context.Users.Include(x => x.PrimaryEmail)
                .FirstOrDefaultAsync(x => (x.Username == user.Login || (x.PrimaryEmail != null && x.PrimaryEmail.Name == user.Login)) && x.Password == user.Password);
            if (existUser == null)
                return NotFound();

            return Ok(await GenerateToken(existUser));
        }

        // POST: api/users/signup
        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<ActionResult<UserAuthResultDto>> SignupUser(UserSignUpDto user)
        {
            if (_context.Users == null)
                return new JsonResult("Something went wrong") { StatusCode = 500 };

            var existUser = await _context.Users.Include(x => x.PrimaryEmail)
                .FirstOrDefaultAsync(x => x.PrimaryEmail != null && x.PrimaryEmail.Name == user.Email && x.Password == user.Password);
            if (existUser != null)
            {
                return BadRequest(new ErrorDto() { Message = "User already exists" });
            }

            // Add user
            var newUser = new User()
            {
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                Password = user.Password,
                About = user.About,
                CreatedAt = DateTime.Now,
                IsBlocked = false,
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            // Add email
            var newEmail = new Email()
            {
                IsVerify = true,
                Name = user.Email,
                UserId = newUser.Id
            };
            await _context.Emails.AddAsync(newEmail);
            await _context.SaveChangesAsync();

            // Set primary email
            newUser.PrimaryEmailId = newEmail.Id;
            await _context.SaveChangesAsync();

            return Ok(await GenerateToken(newUser));
        }

        private async Task<UserAuthResultDto> GenerateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value!);

            var claims = new List<Claim>()
            {
                new Claim(type:"id", value: user.Id.ToString()),
                new Claim(ClaimTypes.Role, "Client"),
                new Claim(JwtRegisteredClaimNames.Sub, user.PrimaryEmail.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.PrimaryEmail.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())

            };

            // Add permissions
            //if (user.Division != null)
            //{
            //    foreach (var permission in user.Division.PermissionsOfDivisions)
            //    {
            //        if (permission.Permission.PermissionName != null)
            //            claims.Add(new Claim(ClaimTypes.Role, permission.Permission.PermissionName));
            //    }
            //}

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TimeSpan.Parse(_configuration.GetSection("JwtConfig:ExpiryTimeFrame").Value!)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return new UserAuthResultDto()
            {
                Token = "Bearer " + jwtToken,
                UserInfo = user.Adapt<UserInfoDto>(),
            };
        }
    }
}
