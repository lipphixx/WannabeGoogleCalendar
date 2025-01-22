using Microsoft.AspNetCore.Mvc;
using WannabeGCalendar.Data;

namespace WannabeGCalendar.Controllers
{
    public class EventsController: Controller
    {
        private readonly ApplicationDbContext dbContext;

        public EventsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult GetAllEvents()
        {
            return Json(dbContext.Events);
        }
    }
}
