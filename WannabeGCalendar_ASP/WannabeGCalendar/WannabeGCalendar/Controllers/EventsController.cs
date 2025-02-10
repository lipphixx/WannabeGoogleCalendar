using Microsoft.AspNetCore.Mvc;
using WannabeGCalendar.Data;
using WannabeGCalendar.Entities;

namespace WannabeGCalendar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public EventsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: api/Events
        [HttpGet(Name = "GetAllEvents")]
        public IActionResult GetAllEvents()
        {
            try
            {
                return Ok(dbContext.Events.ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při načítání událostí: {ex.Message}");
                return StatusCode(500, new { message = "Chyba serveru", error = ex.Message });
            }
        }

        // POST: api/Events
        [HttpPost(Name = "CreateNewEvent")]
        public IActionResult CreateEvent([FromBody] Event newEvent)
        {
            if (newEvent == null)
            {
                return BadRequest("Invalid event data.");
            }

            dbContext.Events.Add(newEvent);
            dbContext.SaveChanges();

            return CreatedAtRoute("GetAllEvents", new { id = newEvent.EventId }, newEvent);
        }

        // DELETE: api/Events/{id}
        [HttpDelete("{id}", Name = "RemoveEvent")]
        public IActionResult RemoveEvent(Int64 id)
        {
            var eventToRemove = dbContext.Events.FirstOrDefault(e => e.EventId == id);
            if (eventToRemove == null)
            {
                return NotFound("Event not found.");
            }

            dbContext.Events.Remove(eventToRemove);
            dbContext.SaveChanges();

            return Ok(eventToRemove);
        }
    }
}
