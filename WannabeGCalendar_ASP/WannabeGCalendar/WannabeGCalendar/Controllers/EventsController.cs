using Microsoft.AspNetCore.Http.HttpResults;
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
        [HttpGet("GetAllEvents")]
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
        
        [HttpPost("CreateNewEvent")]
        public IActionResult CreateEvent([FromBody] Event newEvent)
        {
            if (newEvent == null)
            {
                return BadRequest("Invalid event data.");
            }

            dbContext.Events.Add(newEvent);
            dbContext.SaveChanges();

            return Ok(new { message = "Event byl úspěšně vytvořen", eventId = newEvent.EventId });
        }
        
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

        [HttpPost("UpdateEvent")]
        public IActionResult UpdateEvent([FromBody] Event updatedEvent)
        {
            try
            {
                var eventToUpdate = dbContext.Events.FirstOrDefault(e => e.EventId == updatedEvent.EventId);
                
                eventToUpdate.EventName = updatedEvent.EventName;
                eventToUpdate.EventDate = updatedEvent.EventDate;
                eventToUpdate.EventNote = updatedEvent.EventNote;
                eventToUpdate.EventTime = updatedEvent.EventTime;
                eventToUpdate.ParticipantsIds = updatedEvent.ParticipantsIds;
                eventToUpdate.EventPlace = updatedEvent.EventPlace;
                eventToUpdate.EventLabel = updatedEvent.EventLabel;

                return Ok(dbContext.SaveChanges());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
