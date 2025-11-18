using WebApplication1.Models;

namespace WebApplication1.DTOs
{
    public class SmartBookingResponseDto
    {
        public bool Success { get; set; }
        public Appointment? BookedAppointment { get; set; }
        public List<AlternativeSlot> Alternatives { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}