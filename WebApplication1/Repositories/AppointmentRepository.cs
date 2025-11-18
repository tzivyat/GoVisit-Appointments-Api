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

        public async Task<bool> IsSlotAvailableAsync(string officeId, DateTime dateTime, int durationMinutes)
        {
            var endTime = dateTime.AddMinutes(durationMinutes);
            
            // שאילתה ישירה לבדיקת התנגשות - הרבה יותר מהיר!
            var conflictFilter = Builders<Appointment>.Filter.And(
                Builders<Appointment>.Filter.Eq(a => a.OfficeId, officeId),
                Builders<Appointment>.Filter.Ne(a => a.Status, AppointmentStatus.Cancelled),
                // בדיקת חפיפה ישירה במסד הנתונים
                Builders<Appointment>.Filter.Lt(a => a.AppointmentDate, endTime),
                Builders<Appointment>.Filter.Expr(new BsonDocument("$gt", 
                    new BsonArray { 
                        new BsonDocument("$add", new BsonArray { "$appointmentDate", new BsonDocument("$multiply", new BsonArray { "$durationMinutes", 60000 }) }),
                        dateTime 
                    }))
            );

            // רק בדיקה אם קיים תור מתנגש - לא מביא נתונים
            return !await _appointments.Find(conflictFilter).AnyAsync();
        }

        public async Task<List<DateTime>> GetAvailableSlotsAsync(string officeId, DateTime date, int durationMinutes)
        {
            var startOfDay = date.Date.AddHours(8); // 08:00
            var endOfDay = date.Date.AddHours(17);   // 17:00
            
            // קבלת כל התורים ביום בשאילתה אחת - יעיל יותר!
            var dayFilter = Builders<Appointment>.Filter.And(
                Builders<Appointment>.Filter.Eq(a => a.OfficeId, officeId),
                Builders<Appointment>.Filter.Ne(a => a.Status, AppointmentStatus.Cancelled),
                Builders<Appointment>.Filter.Gte(a => a.AppointmentDate, startOfDay),
                Builders<Appointment>.Filter.Lt(a => a.AppointmentDate, endOfDay)
            );
            
            var existingAppointments = await _appointments.Find(dayFilter)
                .Project(a => new { a.AppointmentDate, a.DurationMinutes })
                .ToListAsync();
            
            var slots = new List<DateTime>();
            
            // בדיקה מקומית מהירה יותר
            for (var time = startOfDay; time.AddMinutes(durationMinutes) <= endOfDay; time = time.AddMinutes(30))
            {
                var endTime = time.AddMinutes(durationMinutes);
                var hasConflict = existingAppointments.Any(apt => 
                    time < apt.AppointmentDate.AddMinutes(apt.DurationMinutes) && 
                    endTime > apt.AppointmentDate);
                
                if (!hasConflict)
                {
                    slots.Add(time);
                }
            }

            return slots;
        }

        public async Task<List<Appointment>> GetFilteredAppointmentsAsync(string officeId, AppointmentFilterDto filter)
        {
            var mongoFilter = Builders<Appointment>.Filter.Eq(a => a.OfficeId, officeId);

            if (filter.ActiveOnly)
            {
                mongoFilter &= Builders<Appointment>.Filter.Ne(a => a.Status, AppointmentStatus.Cancelled);
            }

            if (!string.IsNullOrEmpty(filter.Status) && Enum.TryParse<AppointmentStatus>(filter.Status, true, out var status))
            {
                mongoFilter &= Builders<Appointment>.Filter.Eq(a => a.Status, status);
            }

            if (filter.FromDate.HasValue)
            {
                mongoFilter &= Builders<Appointment>.Filter.Gte(a => a.AppointmentDate, filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                mongoFilter &= Builders<Appointment>.Filter.Lte(a => a.AppointmentDate, filter.ToDate.Value);
            }

            var query = _appointments.Find(mongoFilter);

            // Sorting
            query = filter.SortBy?.ToLower() switch
            {
                "priority" => filter.SortDescending ? query.SortByDescending(a => a.Priority) : query.SortBy(a => a.Priority),
                "status" => filter.SortDescending ? query.SortByDescending(a => a.Status) : query.SortBy(a => a.Status),
                "created" => filter.SortDescending ? query.SortByDescending(a => a.CreatedAt) : query.SortBy(a => a.CreatedAt),
                _ => filter.SortDescending ? query.SortByDescending(a => a.AppointmentDate) : query.SortBy(a => a.AppointmentDate)
            };

            return await query.ToListAsync();
        }

        public async Task<Appointment?> PartialUpdateAsync(string id, UpdateAppointmentDto dto)
        {
            var filter = Builders<Appointment>.Filter.Eq(a => a.Id, id);
            var updateBuilder = Builders<Appointment>.Update.Set(a => a.UpdatedAt, DateTime.UtcNow);

            if (dto.AppointmentDate.HasValue)
                updateBuilder = updateBuilder.Set(a => a.AppointmentDate, dto.AppointmentDate.Value);

            if (!string.IsNullOrEmpty(dto.Status) && Enum.TryParse<AppointmentStatus>(dto.Status, true, out var status))
                updateBuilder = updateBuilder.Set(a => a.Status, status);

            if (!string.IsNullOrEmpty(dto.ServiceType))
                updateBuilder = updateBuilder.Set(a => a.ServiceType, dto.ServiceType);

            if (!string.IsNullOrEmpty(dto.Notes))
                updateBuilder = updateBuilder.Set(a => a.Notes, dto.Notes);

            if (!string.IsNullOrEmpty(dto.Priority) && Enum.TryParse<AppointmentPriority>(dto.Priority, true, out var priority))
                updateBuilder = updateBuilder.Set(a => a.Priority, priority);

            var result = await _appointments.FindOneAndUpdateAsync(filter, updateBuilder,
                new FindOneAndUpdateOptions<Appointment> { ReturnDocument = ReturnDocument.After });

            return result;
        }
    }
}