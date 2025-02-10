using Microsoft.AspNetCore.Mvc;
using WannabeGCalendar.Data;
using WannabeGCalendar.Entities;

namespace WannabeGCalendar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public UsersController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: api/Users
        [HttpGet(Name = "GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                return Ok(dbContext.Users.ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při načítání uživatelů: {ex.Message}");
                return StatusCode(500, new { message = "Chyba serveru", error = ex.Message });
            }
        }
    }
}
