namespace YIL_CSD_Feedback_Management.Helpers
{
    public static class DateTimeHelper
    {
        private const string TimeZoneId = "India Standard Time";

        public static DateTime Now
        {
            get
            {
                return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                    DateTime.UtcNow,
                    TimeZoneId);
            }
        }
    }
}