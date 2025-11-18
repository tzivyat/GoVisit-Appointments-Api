namespace WebApplication1.DTOs
{
    public class UpdateAppointmentDto
    {
        public DateTime? AppointmentDate { get; set; }
        public string? Status { get; set; }
        public string? ServiceType { get; set; }
        public string? Notes { get; set; }
        public string? Priority { get; set; }
    }
}