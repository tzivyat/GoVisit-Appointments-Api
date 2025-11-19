using MongoDB.Driver;
using WebApplication1.Models;
using WebApplication1.DTOs;

namespace WebApplication1.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IMongoCollection<Appointment> _appointments;

        public AppointmentRepository(IMongoDatabase database)
        {
            _appointments = database.GetCollection<Appointment>("appointments");
        }

        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            await _appointments.InsertOneAsync(appointment);
            return appointment;
        }

        public async Task<List<Appointment>> GetByOfficeAsync(string officeId, DateTime? date = null)
        {
            var filter = Builders<Appointment>.Filter.Eq(a => a.OfficeId, officeId);
            
            if (date.HasValue)
            {
                var startOfDay = date.Value.Date;
                var endOfDay = startOfDay.AddDays(1);
                filter &= Builders<Appointment>.Filter.Gte(a => a.AppointmentDate, startOfDay) &
                         Builders<Appointment>.Filter.Lt(a => a.AppointmentDate, endOfDay);
            }

            return await _appointments.Find(filter)
                .SortBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<Appointment?> GetByIdAsync(string id)
        {
            return await _appointments.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Appointment?> UpdateAsync(string id, Appointment appointment)
        {
            var filter = Builders<Appointment>.Filter.Eq(a => a.Id, id);
            var result = await _appointments.ReplaceOneAsync(filter, appointment);
            
            return result.ModifiedCount > 0 ? appointment : null;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<Appointment>.Filter.Eq(a => a.Id, id);
            var result = await _appointments.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<List<Appointment>> GetFilteredAppointmentsAsync(string officeId, AppointmentFilterDto filter)
        {
            var mongoFilter = Builders<Appointment>.Filter.Eq(a => a.OfficeId, officeId);
            return await _appointments.Find(mongoFilter).ToListAsync();
        }

        public async Task<Appointment?> PartialUpdateAsync(string id, UpdateAppointmentDto dto)
        {
            var filter = Builders<Appointment>.Filter.Eq(a => a.Id, id);
            var updateBuilder = Builders<Appointment>.Update.Set(a => a.UpdatedAt, DateTime.UtcNow);
            
            var result = await _appointments.FindOneAndUpdateAsync(filter, updateBuilder,
                new FindOneAndUpdateOptions<Appointment> { ReturnDocument = ReturnDocument.After });
            return result;
        }

        public Task<List<DateTime>> GetAvailableSlotsAsync(string officeId, DateTime date, int durationMinutes)
        {
            return Task.FromResult(new List<DateTime> { date.AddHours(9), date.AddHours(10), date.AddHours(11) });
        }
    }
}