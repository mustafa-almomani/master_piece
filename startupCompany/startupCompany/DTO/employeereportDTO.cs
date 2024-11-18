using startupCompany.Models;

namespace startupCompany.DTO
{
    public class employeereportDTO
    {
    

      
        public string? ReportDetails { get; set; }

        public DateTime? ReportDate { get; set; }

        public int? Completedtask { get; set; }

        public string? PerformanceRating { get; set; }

        public string? Note { get; set; }

        public employeeDTO employee { get; set; }
    }

    public class employeeDTO
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}
