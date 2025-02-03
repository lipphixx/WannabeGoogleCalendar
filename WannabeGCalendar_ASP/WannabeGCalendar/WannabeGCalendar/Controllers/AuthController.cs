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
            User newUser = new(null ,request.Email, request.Password, request.Username, request.FullName, request.PhoneNumber);

            if (newUser != null && 
                dbContext.Users.FirstOrDefault(x => x.Email == newUser.Email) == null && 
                dbContext.Users.FirstOrDefault(x => x.PhoneNumber == newUser.PhoneNumber) == null && 
                dbContext.Users.FirstOrDefault(x => x.Username.ToLower() == newUser.Username.ToLower()) == null)
            {
                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();
                return Ok(newUser);
            }
            return BadRequest("Register Failed!");
        }
            

    }
}
