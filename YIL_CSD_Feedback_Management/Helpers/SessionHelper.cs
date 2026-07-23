using Microsoft.AspNetCore.Http;

namespace YIL_CSD_Feedback_Management.Helpers
{
    public static class SessionHelper
    {
        public static bool IsLoggedIn(HttpContext context)
        {
            return !string.IsNullOrEmpty(
                context.Session.GetString("UserID"));
        }

        public static string UserName(HttpContext context)
        {
            return context.Session.GetString("UserName") ?? "";
        }

        public static string FullName(HttpContext context)
        {
            return context.Session.GetString("FullName") ?? "";
        }

        public static string Role(HttpContext context)
        {
            return context.Session.GetString("Role") ?? "";
        }

        public static string Region(HttpContext context)
        {
            return context.Session.GetString("Region") ?? "";
        }
    }
}