using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs
{
    public class SmartBookingRequestDto
    {
        [Required]
        [StringLength(9, MinimumLength = 9)]
        public string CitizenId { get; set; } = string.Empty;

        [Required]
        public string CitizenName { get; set; } = string.Empty;

        [Required]
        public string CitizenPhone { get; set; } = string.Empty;

        [Required]
        public string OfficeId { get; set; } = string.Empty;

        [Required]
        public string ServiceType { get; set; } = string.Empty;

        [Required]
        public DateTime PreferredDate { get; set; }

        [Required]
        public TimeSpan PreferredTime { get; set; }

        public int DurationMinutes { get; set; } = 30;
        public string Notes { get; set; } = string.Empty;
    }
}