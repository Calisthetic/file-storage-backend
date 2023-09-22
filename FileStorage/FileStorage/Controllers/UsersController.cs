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
using FileStorage.Models.Incoming;

namespace FileStorage.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ApiDbContext _context;

        public UsersController(ApiDbContext context, IMapper mapper,
            IConfiguration configuration, TokenValidationParameters tokenValidationParameters)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
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

        // PUT: api/Users/5
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
          if (_context.Users == null)
          {
              return Problem("Entity set 'ApiDbContext.Users'  is null.");
          }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
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

            var existUser = await _context.Users.Include(x => x.PrimaryEmail)
                .FirstOrDefaultAsync(x => (x.Username == user.Login || x.PrimaryEmail.Name == user.Login) && x.Password == user.Password);
            if (existUser == null)
                return NotFound();

            //return Ok(existUser);
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
                .FirstOrDefaultAsync(x => x.PrimaryEmail.Name == user.Email && x.Password == user.Password);
            if (existUser == null)
                return BadRequest(new ErrorDto()
                {
                    Message = "User already exists"
                });

            return Ok(await GenerateToken(existUser));
        }

        private async Task<UserAuthResultDto> GenerateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value!);

            var claims = new List<Claim>()
            {
                new Claim(type:"Id", value: user.Id.ToString()),
                new Claim(ClaimTypes.Role, "Guest"),
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
                Token = jwtToken,
                UserInfo = user.Adapt<UserInfoDto>(),
            };
        }

        private DateTime UnixTimeStampToDateTime(long unixTimaStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimaStamp).ToUniversalTime();
            return dateTimeVal;
        }

        private string RandomStringGeneration(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPRSTUVWXYZ1234567890abcdefghijklmnoprstuvwxyz_";

            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
