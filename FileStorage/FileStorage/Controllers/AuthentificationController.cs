using FileStorage.Data;
using FileStorage.Models.Db;
using FileStorage.Models.Incoming.User;
using FileStorage.Models.Outcoming;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FileStorage.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/auth")]
    [ApiController]
    public class AuthentificationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApiDbContext _context;

        public AuthentificationController(ApiDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
                Token = "Bearer " + jwtToken
            };
        }
    }
}
