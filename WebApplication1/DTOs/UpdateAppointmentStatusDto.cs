using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs
{
    public class UpdateAppointmentStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;
    }
}