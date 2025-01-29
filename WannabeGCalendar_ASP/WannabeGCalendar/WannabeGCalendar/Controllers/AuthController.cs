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

            return Ok(user);
        }

    }
}
