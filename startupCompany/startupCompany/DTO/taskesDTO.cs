using startupCompany.Models;

namespace startupCompany.DTO
{
    public class taskesDTO
    {





        public int? EmployeeId { get; set; }

        public string? TaskDetails { get; set; }

        public string? TaskStatus { get; set; }

        public DateTime? AssignedDate { get; set; }

        public DateTime? DueDate { get; set; }
    
    }
}
