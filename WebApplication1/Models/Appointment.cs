using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Models
{
    public class Appointment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("citizenId")]
        public string CitizenId { get; set; } = string.Empty;

        [BsonElement("citizenName")]
        public string CitizenName { get; set; } = string.Empty;

        [BsonElement("citizenPhone")]
        public string CitizenPhone { get; set; } = string.Empty;

        [BsonElement("officeId")]
        public string OfficeId { get; set; } = string.Empty;

        [BsonElement("serviceType")]
        public string ServiceType { get; set; } = string.Empty;

        [BsonElement("appointmentDate")]
        public DateTime AppointmentDate { get; set; }

        [BsonElement("status")]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;

        [BsonElement("duration")]
        public int DurationMinutes { get; set; } = 30;

        [BsonElement("priority")]
        public AppointmentPriority Priority { get; set; } = AppointmentPriority.Normal;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum AppointmentPriority
    {
        Low = 1,
        Normal = 2,
        High = 3,
        Urgent = 4
    }

    public enum AppointmentStatus
    {
        Scheduled,
        Confirmed,
        Completed,
        Cancelled,
        NoShow
    }
}