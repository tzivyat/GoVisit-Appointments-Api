namespace WebApplication1.DTOs
{
    public class AppointmentFilterDto
    {
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SortBy { get; set; } = "date";
        public bool SortDescending { get; set; } = false;
        public bool ActiveOnly { get; set; } = true;
    }
}