using System.ComponentModel.DataAnnotations;

namespace prueba.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; } = string.Empty;
    }
}
