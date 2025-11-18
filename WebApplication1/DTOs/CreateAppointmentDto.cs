using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs
{
    public class CreateAppointmentDto
    {
        [Required]
        [StringLength(9, MinimumLength = 9)]
        public string CitizenId { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string CitizenName { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string CitizenPhone { get; set; } = string.Empty;

        [Required]
        public string OfficeId { get; set; } = string.Empty;

        [Required]
        public string ServiceType { get; set; } = string.Empty;

        [Required]
        public DateTime AppointmentDate { get; set; }

        public string Notes { get; set; } = string.Empty;
    }
}