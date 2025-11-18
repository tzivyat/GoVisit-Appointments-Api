namespace WebApplication1.Resources
{
    public static class Messages
    {
        public const string AppointmentCreatedSuccessfully = "התור נקבע בהצלחה";
        public const string SlotNotAvailable = "הזמן המבוקש תפוס. הנה חלופות זמינות:";
        public const string AppointmentNotFound = "תור לא נמצא";
        public const string AppointmentCancelledWithAlternatives = "התור בוטל בהצלחה. הנה תורים חלופיים זמינים:";
        public const string SameDay = "אותו יום";
        public const string DaysOffset = "+{0} ימים";
        public const string DayName = "יום {0}";
        
        // Controller messages
        public const string ErrorCreatingAppointment = "שגיאה ביצירת התור";
        public const string ErrorGettingAppointments = "שגיאה בקבלת התורים";
        public const string ErrorUpdatingAppointment = "שגיאה בעדכון התור";
        public const string ErrorCancellingAppointment = "שגיאה בביטול התור";
        public const string ErrorGettingAppointment = "שגיאה בקבלת התור";
        public const string ErrorSmartBooking = "שגיאה בזימון התור החכם";
        public const string ErrorGettingPrioritizedAppointments = "שגיאה בקבלת התורים המסוננים";
    }
}