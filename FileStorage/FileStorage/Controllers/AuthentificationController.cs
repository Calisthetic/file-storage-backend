using Asp.Versioning;
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
    [AllowAnonymous]
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
        [HttpPost("signup")]
        public async Task<ActionResult<UserAuthResultDto>> SignupUser(UserSignUpDto user)
        {
            if (_context.Users == null)
                return new JsonResult("Something went wrong") { StatusCode = 500 };

            var existUser = await _context.Users.Include(x => x.PrimaryEmail)
                .FirstOrDefaultAsync(x => x.PrimaryEmail != null && x.PrimaryEmail.Name == user.Email && x.Password == user.Password);
            if (existUser != null)
            {
                return BadRequest(new { Message = "User already exists" });
            }

            var verifyCode = string.Empty;
            while (string.IsNullOrEmpty(verifyCode))
            {
                string code = RandomStringGeneration(16);
                var existedUser = await _context.Users.FirstOrDefaultAsync(x => x.VerifyCode == code);
                if (existedUser == null)
                {
                    verifyCode = code;
                }
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
                VerifyCode = verifyCode,
                IsVerify = false
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            // Add email
            var newEmail = new Email()
            {
                IsVerify = false,
                Name = user.Email,
                UserId = newUser.Id
            };
            await _context.Emails.AddAsync(newEmail);
            await _context.SaveChangesAsync();

            // Set primary email
            newUser.PrimaryEmailId = newEmail.Id;
            await _context.SaveChangesAsync();

            // Send email
            var client = new HttpClient();
            var values = new Dictionary<string, string>
            {
                { "email", newEmail.Name },
                { "url", "https://file-storage-frontend.vercel.app/verify/user/" + verifyCode }
            };

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("http://www.example.com/recepticle.aspx", content);

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
            if (user.Id == 1)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

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


        
        [HttpPatch("verify/{code}")]
        public async Task<IActionResult> VerifyUserSignup(string code)
        {
            var currentUser = await _context.Users.Include(x => x.PrimaryEmail).FirstOrDefaultAsync(x => x.VerifyCode == code);
            if (currentUser == null || currentUser.PrimaryEmail is null)
            {
                return NotFound();
            }

            currentUser.IsVerify = true;
            currentUser.VerifyCode = null;
            currentUser.PrimaryEmail.IsVerify = true;
            await _context.SaveChangesAsync();

            return Ok(await GenerateToken(currentUser));
        }

        private string RandomStringGeneration(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPRSTUVWXYZ1234567890abcdefghijklmnoprstuvwxyz_";

            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
