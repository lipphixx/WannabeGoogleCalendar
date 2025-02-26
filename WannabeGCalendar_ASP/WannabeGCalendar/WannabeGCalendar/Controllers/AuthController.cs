using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WannabeGCalendar.Data;
using WannabeGCalendar.Entities;

namespace WannabeGCalendar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public AuthController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Email and password cannot be empty.");
            }

            var user = dbContext.Users.FirstOrDefault(x => x.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ThisIsA32CharacterLongKey1234567890!");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString, UserId = user.UserId, Username = user.Username });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] ApplicationRegisterRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) ||
                string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.FullName) || string.IsNullOrEmpty(request.PhoneNumber))
            {
                return BadRequest("Missing required fields.");
            }

            if (dbContext.Users.Any(x => x.Email == request.Email)) return BadRequest("Email already exists!");
            if (dbContext.Users.Any(x => x.PhoneNumber == request.PhoneNumber)) return NotFound("Phone number already exists!");
            if (dbContext.Users.Any(x => x.Username.ToLower() == request.Username.ToLower())) return Unauthorized("Username already exists!");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            User newUser = new User(null, request.Email, hashedPassword, request.Username, request.FullName, request.PhoneNumber);

            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();

            // Vložení nového uživatele bez použití nesprávného názvu argumentu
            return Ok(newUser);
        }

        [HttpPost("CreateNewRecovery")]
        public IActionResult CreateNewRecovery([FromBody] Recovery r)
        {
            if (string.IsNullOrEmpty(r.RecoveryEmail) || string.IsNullOrEmpty(r.RecoveryCode))
            {
                return BadRequest("Recovery email and code cannot be empty.");
            }

            r.ExpiresAt = DateTime.UtcNow.AddMinutes(10); // Expirace po 10 minutách
            dbContext.Recoveries.Add(r);
            dbContext.SaveChanges();
            return Ok(new { message = "Recovery was successfully created", r });
        }

        [HttpPost("RemoveRecovery")]
        public IActionResult RemoveRecovery([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email cannot be empty.");
            }

            var recoveries = dbContext.Recoveries.Where(x => x.RecoveryEmail == email).ToList();
            recoveries.ForEach(x => dbContext.Recoveries.Remove(x));
            dbContext.SaveChanges();
            return Ok(email);
        }

        [HttpPost("CheckCode")]
        public IActionResult CheckCode([FromBody] Recovery r)
        {
            if (string.IsNullOrEmpty(r.RecoveryEmail) || string.IsNullOrEmpty(r.RecoveryCode))
            {
                return BadRequest("Recovery email and code cannot be empty.");
            }

            var rec = dbContext.Recoveries.FirstOrDefault(x => x.RecoveryEmail == r.RecoveryEmail && 
                                                               x.RecoveryCode == r.RecoveryCode && 
                                                               x.ExpiresAt > DateTime.UtcNow);
            if (rec != null)
            {
                RemoveRecovery(r.RecoveryEmail);
                return Ok();
            }
            return BadRequest("Invalid or expired recovery code.");
        }

        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest("Email and new password cannot be empty.");
            }

            var user = dbContext.Users.FirstOrDefault(x => x.Email == request.Email);
            if (user == null)
            {
                return NotFound("User not found!");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            dbContext.SaveChanges();
            return Ok();
        }

        public class ChangePasswordRequest
        {
            public string Email { get; set; }
            public string NewPassword { get; set; }
        }
    }
}