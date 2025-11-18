using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Resources;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IAppointmentService appointmentService, ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        /// <summary>
        /// זימון תור חכם עם בדיקת זמינות ואישור אוטומטי
        /// </summary>
        [HttpPost("smart-booking")]
        public async Task<ActionResult<SmartBookingResponseDto>> SmartBookAppointment(
            [FromBody] SmartBookingRequestDto request)
        {
            try
            {
                var result = await _appointmentService.SmartBookAppointmentAsync(request);
                
                if (result.Success)
                {
                    return CreatedAtAction(nameof(GetAppointment), 
                        new { id = result.BookedAppointment!.Id }, result);
                }
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in smart booking");
                return BadRequest(Messages.ErrorSmartBooking);
            }
        }

        /// <summary>
        /// רשימת תורים לפי סדר עדיפויות וסינון מתקדם
        /// </summary>
        [HttpPost("office/{officeId}/search")]
        public async Task<ActionResult<List<Appointment>>> GetPrioritizedAppointments(
            string officeId,
            [FromBody] AppointmentFilterDto filter)
        {
            try
            {
                var appointments = await _appointmentService.GetPrioritizedAppointmentsAsync(officeId, filter);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting prioritized appointments for office {OfficeId}", officeId);
                return BadRequest(Messages.ErrorGettingPrioritizedAppointments);
            }
        }

        /// <summary>
        /// עדכון מספר שדות בתור אחד
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<ActionResult<Appointment>> UpdateAppointmentFields(
            string id,
            [FromBody] UpdateAppointmentDto dto)
        {
            try
            {
                var appointment = await _appointmentService.UpdateAppointmentFieldsAsync(id, dto);
                if (appointment == null)
                    return NotFound(Messages.AppointmentNotFound);

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment fields {Id}", id);
                return BadRequest(Messages.ErrorUpdatingAppointment);
            }
        }

        /// <summary>
        /// ביטול תור עם הצעת תור חלופי
        /// </summary>
        [HttpDelete("{id}/with-alternative")]
        public async Task<ActionResult<SmartBookingResponseDto>> CancelWithAlternative(string id)
        {
            try
            {
                var result = await _appointmentService.CancelWithAlternativeAsync(id);
                
                if (!result.Success && result.Message.Contains("לא נמצא"))
                    return NotFound(result.Message);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling appointment with alternative {Id}", id);
                return BadRequest(Messages.ErrorCancellingAppointment);
            }
        }

        /// <summary>
        /// קבלת תור לפי מזהה - עזר למתודות אחרות
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(string id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                    return NotFound(Messages.AppointmentNotFound);

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointment {Id}", id);
                return BadRequest(Messages.ErrorGettingAppointment);
            }
        }
    }
}