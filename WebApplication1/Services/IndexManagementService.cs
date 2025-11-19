using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class IndexManagementService
    {
        private readonly IMongoCollection<Appointment> _appointments;

        public IndexManagementService(IMongoDatabase database)
        {
            _appointments = database.GetCollection<Appointment>("appointments");
        }

        public async Task EnsureIndexesAsync()
        {
            // Index פשוט - משרד + זמן
            var indexKeys = Builders<Appointment>.IndexKeys
                .Ascending(a => a.OfficeId)
                .Ascending(a => a.AppointmentDate);
            
            var indexOptions = new CreateIndexOptions 
            { 
                Unique = true,
                Name = "unique_appointment_slot"
            };
            
            try
            {
                await _appointments.Indexes.CreateOneAsync(
                    new CreateIndexModel<Appointment>(indexKeys, indexOptions));
            }
            catch (MongoCommandException)
            {
                // Index כבר קיים
            }
        }

        public async Task<List<BsonDocument>> GetIndexesAsync()
        {
            return await _appointments.Indexes.List().ToListAsync();
        }

        public async Task DropIndexAsync(string indexName)
        {
            await _appointments.Indexes.DropOneAsync(indexName);
        }
    }
}