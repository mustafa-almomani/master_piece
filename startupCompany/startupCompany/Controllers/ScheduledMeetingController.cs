using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using startupCompany.DTO;
using startupCompany.helper;
using startupCompany.Models;

namespace startupCompany.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduledMeetingController : ControllerBase
    {
        private readonly MyDbContext _Db;
        private readonly EmailHelper _emailHelper;
        public ScheduledMeetingController(MyDbContext db, EmailHelper emailHelper)
        {
            _Db = db;
            _emailHelper = emailHelper;
        }

        [HttpPost("adminRespone")]
        public async Task<IActionResult> AdminRespone([FromForm] ScheduledMeetingDTO adminRes)
        {
            try
            {
                // تخزين البيانات في قاعدة البيانات
                var client = new ClientMeeting
                {
                    ClientName = adminRes.ClientName,
                    ClientEmail = adminRes.Email,
                    ClientPhone = adminRes.ClientPhone,
                    MeetingDateTime = adminRes.MeetingDate,
                    MeetingLink = adminRes.MeetingLink, // أو يمكنك استخدام الرابط الثابت إذا كان لديك رابط معين
                    MeetingPlatform = adminRes.MeetingPlatform,
                    Subjectmeet = adminRes.desc,
                };

                // إضافة البيانات إلى قاعدة البيانات
                _Db.ClientMeetings.Add(client);
                _Db.SaveChanges();

                // إعداد البريد الإلكتروني
                string email = adminRes.Email;
                string subject = "Scheduling a Meeting to Discuss the Upcoming IT Project";
                string desc = adminRes.desc;
                string platform = adminRes.MeetingPlatform;
                string link = adminRes.MeetingLink;
                DateTime date = adminRes.MeetingDate;

                // تنسيق البريد الإلكتروني
                string emailBody = $@"
Dear {adminRes.ClientName},

We would like to schedule a meeting to discuss the upcoming IT project. Below are the details of the meeting:

Subject: {desc}

Platform: {platform}
Meeting Link: {link}
Date: {date.ToString("f")}

Please ensure to be available at the scheduled time, and feel free to reach out if you have any questions.

Best regards,
[StartUp Company ]
";

                // إرسال البريد الإلكتروني مع اسم الشخص في "to" بدلاً من "to user"
                _emailHelper.SendMessage(
                    adminRes.ClientName,  // هنا يتم استبدال "to user" باسم العميل
                    email,
                    subject,
                    emailBody
                );

                return Ok("Data saved and email sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return BadRequest("An error occurred while processing your request");
            }
        }


        [HttpPost("employeemeeting")]
        public IActionResult EmployeeMeeting([FromForm] employeemeeting DTO)
        {
            var meetings = new List<TeamMeeting>();

            foreach (var employeeId in DTO.EmployeeId)
            {
                try
                {
                    // استرجاع الموظف من قاعدة البيانات باستخدام EmployeeId
                    var employee = _Db.Employees.FirstOrDefault(e => e.EmployeeId == employeeId);

                    if (employee == null)
                    {
                        return NotFound($"Employee with ID {employeeId} not found.");
                    }

                    // إنشاء الاجتماع لكل موظف
                    var employeemeet = new TeamMeeting
                    {
                        EmployeeId = employeeId,
                        MeetingAgenda = DTO.MeetingAgenda,
                        MeetingDateTime = DTO.MeetingDateTime,
                        Linkmeeting = "https://us05web.zoom.us/j/8151540806?pwd=7nGweNhS3FyLg4adlakxaLjY7d1INi.1",
                    };

                    meetings.Add(employeemeet);
                    _Db.TeamMeetings.Add(employeemeet);

                    // إرسال بريد إلكتروني لكل موظف مع تفاصيل الاجتماع
                    _emailHelper.SendMessage("to employee", employee.Email, "Meeting Scheduled",
                        $"Dear {employee.FirstName},\n\nYour meeting has been scheduled for {DTO.MeetingDateTime}.\nPlease find the meeting link: {employeemeet.Linkmeeting}.\n\nBest regards.");
                }
                catch (Exception ex)
                {
                    // تسجيل الخطأ بالتفصيل
                    Console.WriteLine($"Error occurred for employee with ID {employeeId}: {ex.Message}");

                    // إرسال استجابة 500 مع تفاصيل الخطأ
                    return StatusCode(500, $"An error occurred for employee ID {employeeId}: {ex.Message}");
                }
            }

            try
            {
                // حفظ الاجتماعات في قاعدة البيانات
                _Db.SaveChanges();
            }
            catch (Exception dbEx)
            {
                // إذا حدث خطأ أثناء حفظ البيانات في قاعدة البيانات
                Console.WriteLine($"Error saving meetings to the database: {dbEx.Message}");
                return StatusCode(500, $"Database error: {dbEx.Message}");

            }

            return Ok();
        }







    }

}
