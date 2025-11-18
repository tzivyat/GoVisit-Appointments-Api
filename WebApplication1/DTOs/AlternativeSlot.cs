namespace WebApplication1.DTOs
{
    public class AlternativeSlot
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}