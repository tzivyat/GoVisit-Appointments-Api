using WebApplication1.Models;
using WebApplication1.DTOs;

namespace WebApplication1.Repositories
{
    public interface IAppointmentRepository
    {
        Task<Appointment> CreateAsync(Appointment appointment);
        Task<List<Appointment>> GetByOfficeAsync(string officeId, DateTime? date = null);
        Task<Appointment?> GetByIdAsync(string id);
        Task<Appointment?> UpdateAsync(string id, Appointment appointment);
        Task<bool> DeleteAsync(string id);
        Task<List<DateTime>> GetAvailableSlotsAsync(string officeId, DateTime date, int durationMinutes);
        Task<List<Appointment>> GetFilteredAppointmentsAsync(string officeId, AppointmentFilterDto filter);
        Task<Appointment?> PartialUpdateAsync(string id, UpdateAppointmentDto dto);
    }
}