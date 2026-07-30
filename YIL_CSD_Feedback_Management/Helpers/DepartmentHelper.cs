namespace YIL_CSD_Feedback_Management.Helpers
{
    public static class DepartmentHelper
    {
        public static string GetDepartmentName(int departmentId)
        {
            return departmentId switch
            {
                1 => "Site Service",
                2 => "Training",
                3 => "Bench Repair",
                _ => "Customer Feedback"
            };
        }

        public static string GetPortalTitle(int departmentId)
        {
            return departmentId switch
            {
                1 => "Site Service Feedback Management",
                2 => "Training Feedback Management",
                3 => "Bench Repair Feedback Management",
                _ => "Customer Feedback Management"
            };
        }

        public static string GetDashboardTitle(int departmentId)
        {
            return departmentId switch
            {
                1 => "Site Service Dashboard",
                2 => "Training Dashboard",
                3 => "Bench Repair Dashboard",
                _ => "Dashboard"
            };
        }

        public static string GetModuleName(int departmentId)
        {
            return departmentId switch
            {
                1 => "Site Service",
                2 => "Training",
                3 => "Bench Repair",
                _ => "Customer Feedback"
            };
        }
    }
}