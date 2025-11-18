using WebApplication1.Models;
using WebApplication1.DTOs;
using WebApplication1.Repositories;
using WebApplication1.Resources;

namespace WebApplication1.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repository;

        public AppointmentService(IAppointmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<SmartBookingResponseDto> SmartBookAppointmentAsync(SmartBookingRequestDto request)
        {
            var requestedDateTime = request.PreferredDate.Date.Add(request.PreferredTime);
            
            var isAvailable = await _repository.IsSlotAvailableAsync(
                request.OfficeId, requestedDateTime, request.DurationMinutes);

            if (isAvailable)
            {
                var appointment = new Appointment
                {
                    CitizenId = request.CitizenId,
                    CitizenName = request.CitizenName,
                    CitizenPhone = request.CitizenPhone,
                    OfficeId = request.OfficeId,
                    ServiceType = request.ServiceType,
                    AppointmentDate = requestedDateTime,
                    DurationMinutes = request.DurationMinutes,
                    Notes = request.Notes,
                    Status = AppointmentStatus.Confirmed,
                    CreatedAt = DateTime.UtcNow
                };

                var createdAppointment = await _repository.CreateAsync(appointment);
                
                return new SmartBookingResponseDto
                {
                    Success = true,
                    BookedAppointment = createdAppointment,
                    Message = Messages.AppointmentCreatedSuccessfully
                };
            }

            var alternatives = await FindAlternativeSlotsAsync(
                request.OfficeId, requestedDateTime, request.DurationMinutes);

            return new SmartBookingResponseDto
            {
                Success = false,
                Alternatives = alternatives,
                Message = Messages.SlotNotAvailable
            };
        }

        public async Task<List<Appointment>> GetPrioritizedAppointmentsAsync(string officeId, AppointmentFilterDto filter)
        {
            return await _repository.GetFilteredAppointmentsAsync(officeId, filter);
        }

        public async Task<Appointment?> UpdateAppointmentFieldsAsync(string id, UpdateAppointmentDto dto)
        {
            return await _repository.PartialUpdateAsync(id, dto);
        }

        public async Task<SmartBookingResponseDto> CancelWithAlternativeAsync(string id)
        {
            var appointment = await _repository.GetByIdAsync(id);
            if (appointment == null)
            {
                return new SmartBookingResponseDto
                {
                    Success = false,
                    Message = Messages.AppointmentNotFound
                };
            }

            appointment.Status = AppointmentStatus.Cancelled;
            await _repository.UpdateAsync(id, appointment);

            var weekStart = appointment.AppointmentDate.Date.AddDays(-(int)appointment.AppointmentDate.DayOfWeek);
            var weekEnd = weekStart.AddDays(7);
            
            var alternatives = new List<AlternativeSlot>();
            
            for (var date = weekStart; date < weekEnd; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
                    continue;

                var availableSlots = await _repository.GetAvailableSlotsAsync(
                    appointment.OfficeId, date, appointment.DurationMinutes);
                
                alternatives.AddRange(availableSlots.Take(2).Select(slot => new AlternativeSlot
                {
                    Date = slot.Date,
                    Time = slot.TimeOfDay,
                    Reason = date == appointment.AppointmentDate.Date ? Messages.SameDay : string.Format(Messages.DayName, date.ToString("dddd"))
                }));

                if (alternatives.Count >= 3) break;
            }

            return new SmartBookingResponseDto
            {
                Success = true,
                Alternatives = alternatives.Take(3).ToList(),
                Message = Messages.AppointmentCancelledWithAlternatives
            };
        }

        private async Task<List<AlternativeSlot>> FindAlternativeSlotsAsync(
            string officeId, DateTime requestedDateTime, int durationMinutes)
        {
            var alternatives = new List<AlternativeSlot>();
            var searchDate = requestedDateTime.Date;
            
            for (int dayOffset = 0; dayOffset <= 7 && alternatives.Count < 3; dayOffset++)
            {
                var currentDate = searchDate.AddDays(dayOffset);
                
                if (currentDate.DayOfWeek == DayOfWeek.Friday || currentDate.DayOfWeek == DayOfWeek.Saturday)
                    continue;

                var availableSlots = await _repository.GetAvailableSlotsAsync(
                    officeId, currentDate, durationMinutes);

                foreach (var slot in availableSlots.Take(3 - alternatives.Count))
                {
                    alternatives.Add(new AlternativeSlot
                    {
                        Date = slot.Date,
                        Time = slot.TimeOfDay,
                        Reason = dayOffset == 0 ? Messages.SameDay : string.Format(Messages.DaysOffset, dayOffset)
                    });
                }
            }

            return alternatives;
        }
    }
}