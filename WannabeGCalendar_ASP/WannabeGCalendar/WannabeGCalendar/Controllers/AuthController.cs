using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
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
            User user = dbContext.Users.FirstOrDefault(x => x.Email == request.Email && x.Password == request.Password);
            if (user == null)
            {
                return Unauthorized("Login Failed!");
            }

            return Ok(user.UserId);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] ApplicationRegisterRequest request)
        {
            Console.WriteLine($"Received request: Email={request.Email}, Password={request.Password}, Username={request.Username}, FullName={request.FullName}, PhoneNumber={request.PhoneNumber}");

            if (request == null)
            {
                return BadRequest("Request body is null!");
            }

            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) ||
                string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.FullName) ||
                string.IsNullOrEmpty(request.PhoneNumber))
            {
                return BadRequest("Missing required fields!");
            }

            User newUser = new(null, request.Email, request.Password, request.Username, request.FullName, request.PhoneNumber);

            if (dbContext.Users.Any(x => x.Email == newUser.Email))
                return BadRequest("Email already exists!");

            if (dbContext.Users.Any(x => x.PhoneNumber == newUser.PhoneNumber))
                return BadRequest("Phone number already exists!");

            if (dbContext.Users.Any(x => x.Username.ToLower() == newUser.Username.ToLower()))
                return BadRequest("Username already exists!");

            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();
            return Ok(newUser);
        }



    }
}
