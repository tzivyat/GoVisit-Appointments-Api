using WebApplication1.Models;
using WebApplication1.DTOs;

namespace WebApplication1.Services
{
    public interface IAppointmentService
    {
        Task<Appointment?> GetAppointmentByIdAsync(string id);
        Task<SmartBookingResponseDto> SmartBookAppointmentAsync(SmartBookingRequestDto request);
        Task<List<Appointment>> GetPrioritizedAppointmentsAsync(string officeId, AppointmentFilterDto filter);
        Task<Appointment?> UpdateAppointmentFieldsAsync(string id, UpdateAppointmentDto dto);
        Task<SmartBookingResponseDto> CancelWithAlternativeAsync(string id);
    }
}