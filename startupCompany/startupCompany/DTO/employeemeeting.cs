using startupCompany.Models;

namespace startupCompany.DTO
{
    public class employeemeeting
    {


        public List<int> EmployeeId { get; set; }

        public string? MeetingAgenda { get; set; }

        public DateTime? MeetingDateTime { get; set; }

        public string? Linkmeeting { get; set; }
    }
}
