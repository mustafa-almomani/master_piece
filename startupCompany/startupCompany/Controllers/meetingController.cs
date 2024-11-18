using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using startupCompany.DTO;
using startupCompany.Models;

namespace startupCompany.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class meetingController : ControllerBase
    {
        private readonly MyDbContext _Db;

        public meetingController(MyDbContext db)
        {
            _Db = db;
        }

        [HttpPost("newmeetingwithclient")]
        public IActionResult newmeetingwithclient([FromForm] clientmeetingDTO DTO)
        {
            var client = new ClientMeeting
            {
                ClientName = DTO.ClientName,
                ClientEmail = DTO.ClientEmail,
                ClientPhone = DTO.ClientPhone,
                MeetingDateTime = DTO.MeetingDateTime,
                MeetingLink = "https://us05web.zoom.us/j/8151540806?pwd=7nGweNhS3FyLg4adlakxaLjY7d1INi.1",
                MeetingPlatform = DTO.MeetingPlatform,
                Subjectmeet = DTO.Subjectmeet,
            };
            _Db.ClientMeetings.Add(client);
            _Db.SaveChanges();
            return Ok(client);
        }

        [HttpPost("employeemeeting")]
        public IActionResult EmployeeMeeting([FromForm] employeemeeting DTO)
        {
            var meetings = new List<TeamMeeting>();

            foreach (var employeeId in DTO.EmployeeId)
            {
                var employeemeet = new TeamMeeting
                {
                    EmployeeId = employeeId,
                    MeetingAgenda = DTO.MeetingAgenda,
                    MeetingDateTime = DTO.MeetingDateTime,
                    Linkmeeting = "https://us05web.zoom.us/j/8151540806?pwd=7nGweNhS3FyLg4adlakxaLjY7d1INi.1",
                };

                meetings.Add(employeemeet);
                _Db.TeamMeetings.Add(employeemeet);
            }

            _Db.SaveChanges();

            return Ok(meetings);
        }


        [HttpGet("getemployees")]
        public IActionResult GetEmployees()
        {
            // افترض أن لديك جدولًا يحتوي على بيانات الموظفين
            var employees = _Db.Employees
                .Select(e => new
                {
                    EmployeeId = e.EmployeeId,
                    FullName = e.FirstName + " " + e.LastName
                })
                .ToList();

            return Ok(employees);
        }

        [HttpGet("getallmeeting")]
        public IActionResult GetAllMeetings()
        {
            // جلب بيانات اجتماعات الموظفين
            var employeeMeetings = _Db.TeamMeetings
                .Select(e => new
                {
                    MeetingId = e.MeetingId,
                    EmployeeId = e.EmployeeId,
                    EmployeeName = _Db.Employees
                        .Where(emp => emp.EmployeeId == e.EmployeeId)
                        .Select(emp => emp.FirstName + " " + emp.LastName + " ")
                        .FirstOrDefault(),
                    MeetingAgenda = e.MeetingAgenda,
                    MeetingDateTime = e.MeetingDateTime,
                    meetinglink = e.Linkmeeting
                }).ToList();

            // تجميع الموظفين حسب موعد الاجتماع
            var groupedEmployeeMeetings = employeeMeetings
                .GroupBy(m => m.MeetingDateTime)
                .Select(g => new
                {
                    MeetingDateTime = g.Key,
                    Employees = g.Select(emp => emp.EmployeeName).ToList(),
                    MeetingAgenda = g.FirstOrDefault().MeetingAgenda,
                    Linkmeeting = g.FirstOrDefault().meetinglink
                }).ToList();

            // جلب بيانات اجتماعات العملاء
            var customerMeetings = _Db.ClientMeetings
                .Select(c => new
                {
                    MeetingId = c.MeetingId,
                    CustomerId = c.ClientName,
                    MeetingAgenda = c.Subjectmeet,
                    MeetingDateTime = c.MeetingDateTime,
                    meetinglink = c.MeetingLink
                }).ToList();

            // دمج البيانات وإرجاعها
            var allMeetings = new
            {
                EmployeeMeetings = groupedEmployeeMeetings, // الاجتماعات المجموعة
                CustomerMeetings = customerMeetings
            };

            return Ok(allMeetings);
        }

        [HttpDelete("deletemeetbydatetime/{meetingDateTime}")]
        public IActionResult DeleteMeetingsByDateTime(string meetingDateTime)
        {
            // محاولة تحويل meetingDateTime من نص إلى DateTime
            if (DateTime.TryParse(meetingDateTime, out DateTime parsedDateTime))
            {
                // محاولة حذف اجتماعات العملاء التي تحدث في نفس وقت الاجتماع
                var clientMeetings = _Db.ClientMeetings.Where(x => x.MeetingDateTime == parsedDateTime).ToList();
                if (clientMeetings.Any())
                {
                    _Db.ClientMeetings.RemoveRange(clientMeetings); // حذف جميع اجتماعات العملاء في نفس الوقت
                }

                // محاولة حذف اجتماعات الموظفين التي تحدث في نفس وقت الاجتماع
                var employeeMeetings = _Db.TeamMeetings.Where(x => x.MeetingDateTime == parsedDateTime).ToList();
                if (employeeMeetings.Any())
                {
                    _Db.TeamMeetings.RemoveRange(employeeMeetings); // حذف جميع اجتماعات الموظفين في نفس الوقت
                }

                // حفظ التغييرات
                _Db.SaveChanges();

                // إرجاع استجابة 204 No Content إذا تم الحذف بنجاح
                return NoContent();
            }
            else
            {
                // إرجاع استجابة خطأ إذا كان التنسيق غير صحيح
                return BadRequest("Invalid date format.");
            }
        }

        

   


    }
}
