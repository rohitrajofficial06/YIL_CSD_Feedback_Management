using System.Security.Claims;

namespace YIL_CSD_Feedback_Management.Helpers
{
    public static class CurrentUserHelper
    {
        public static int GetDepartmentId(ClaimsPrincipal user)
        {
            var value = user.FindFirst("DepartmentId")?.Value;

            return int.TryParse(value, out int departmentId)
                ? departmentId
                : 1;
        }

        public static string GetRegion(ClaimsPrincipal user)
        {
            return user.FindFirst("Region")?.Value ?? "";
        }

        public static string GetRole(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }
    }
}