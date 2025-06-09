using Microsoft.AspNetCore.Mvc;
using prueba.Data;
using prueba.Models;

namespace prueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("test-db")]
        public IActionResult TestDatabaseConnection()
        {
            try
            {
                bool canConnect = _context.Database.CanConnect();
                if (canConnect)
                    return Ok(new { DatabaseConnected = true });
                else
                    return StatusCode(500, new { DatabaseConnected = false });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { DatabaseConnected = false, Error = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Appointment>> Get()
        {
            return _context.Appointments.ToList();
        }

        [HttpPost]
        public IActionResult Post(Appointment appointment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!IsOfficeHours(appointment.Date, appointment.StartTime))
                return BadRequest("Appointments are only allowed between 9:00 and 18:00, Monday to Friday.");

            var sameDayAppointments = _context.Appointments
                .Where(a => a.Date == appointment.Date)
                .ToList();

            var exists = sameDayAppointments.Any(a =>
                (appointment.StartTime > a.StartTime && appointment.StartTime < a.StartTime.AddHours(1)) ||
                (appointment.StartTime > a.StartTime.AddHours(-1) && appointment.StartTime < a.StartTime)

            );

            if (exists)
                return Conflict("An appointment is already scheduled within 1 hour of that time.");

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return CreatedAtAction(nameof(Get), new { id = appointment.Id }, appointment);
        }

        [HttpGet("taken")]
        public IActionResult GetTakenAppointments([FromQuery] int year, [FromQuery] int month)
        {
            if (year < 1 || month < 1 || month > 12)
                return BadRequest("Invalid year or month.");

            var takenAppointments = _context.Appointments
                .Where(a => a.Date.Year == year && a.Date.Month == month)
                .GroupBy(a => a.Date)
                .ToDictionary(
                    g => g.Key.ToString("yyyy-MM-dd"),
                    g => g.Select(a => a.StartTime.ToString("HH:mm")).ToList()
                );

            return Ok(takenAppointments);
        }



        /* METODOS PRIVADOS */
        private bool IsOfficeHours(DateOnly date, TimeOnly startTime)
        {
            // Solo lunes a viernes
            var dayOfWeek = date.DayOfWeek;
            if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
                return false;

            // Entre 09:00 y 18:00
            var start = new TimeOnly(9, 0);
            var end = new TimeOnly(18, 0);

            return startTime >= start && startTime <= end;
        }


    }
}
