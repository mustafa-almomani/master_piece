using startupCompany.Models;

namespace startupCompany.DTO
{
    public class addtaskforemployeeDTO
    {
        public string? TaskDetails { get; set; }

        public string? TaskStatus { get; set; }

        public List<EmployeeDTO> Employees { get; set; } // تعديل: قائمة من الموظفين

        public RequestDTO Request { get; set; } // طلب واحد فقط
    }

    public class RequestDTO
    {
        public int RequestId { get; set; }
        public string? ProjectDetails { get; set; }
        public DateTime? RequestDate { get; set; }
    }

    public class EmployeeDTO
    {
        public int EmployeeId { get; set; } // معرف الموظف
    }
}
