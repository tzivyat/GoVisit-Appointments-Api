using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Models
{
    public class Office
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("ministry")]
        public string Ministry { get; set; } = string.Empty;

        [BsonElement("address")]
        public string Address { get; set; } = string.Empty;

        [BsonElement("city")]
        public string City { get; set; } = string.Empty;

        [BsonElement("services")]
        public List<string> Services { get; set; } = new();

        [BsonElement("workingHours")]
        public WorkingHours WorkingHours { get; set; } = new();

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
    }

    public class WorkingHours
    {
        [BsonElement("startTime")]
        public TimeSpan StartTime { get; set; } = new(8, 0, 0);

        [BsonElement("endTime")]
        public TimeSpan EndTime { get; set; } = new(16, 0, 0);

        [BsonElement("workingDays")]
        public List<DayOfWeek> WorkingDays { get; set; } = new() { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday };
    }
}