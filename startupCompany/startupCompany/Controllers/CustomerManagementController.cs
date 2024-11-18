using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPal.Api;
using startupCompany.DTO;
using startupCompany.helper;
using startupCompany.Models;
using Stripe;
using System.Net.Mail;

namespace startupCompany.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerManagementController : ControllerBase
    {
        private readonly MyDbContext _Db;
        private readonly PayPalPaymentService _paymentService;
        private readonly string _redirectUrl;
        private readonly EmailHelper _emailHelper;
        public CustomerManagementController(MyDbContext db, IConfiguration config, PayPalPaymentService paymentService, EmailHelper emailHelper)
        {
            _Db = db;
            _paymentService = paymentService;
            _redirectUrl = config["PayPal:RedirectUrl"];
            _emailHelper = emailHelper;
        }

        [HttpGet("getallorder")]
        public IActionResult GetAllOrders()
        {
            // استخدام Include لجلب بيانات المستخدم، الخدمة، والموظف المرتبطين بالطلب
            var orders = _Db.CustomerRequests
                            .Include(cr => cr.User)    // جلب بيانات المستخدم المرتبط
                            .Include(cr => cr.Service)
                            /*.Include(cr => cr.Payments)*/// جلب بيانات الخدمة المرتبطة
                                                       // جلب بيانات الموظف المسؤول (على افتراض وجود علاقة)
                            .AsEnumerable()            // لتحويل الاستعلام إلى ذاكرة لاستخدام الفهرس
                            .GroupBy(cr => new { cr.User.UserId, cr.User.FirstName, cr.User.LastName, cr.User.Email, cr.User.PhoneNumber }) // تجميع حسب المستخدم
                            .Select(group => new
                            {
                                UserId = group.Key.UserId,

                                UserFullName = group.Key.FirstName + " " + group.Key.LastName, // الاسم الكامل للمستخدم
                                EmployeeFullName = group.Key.FirstName + " " + group.Key.LastName,
                                email = group.Key.Email,// الاسم الكامل للموظف المسؤول
                                PhoneNumber = group.Key.PhoneNumber,
                                TotalOrders = group.Where(x => x.Status == "In Progress").Count(), // مجموع الطلبات لهذا المستخدم
                                Orders = group.Select(cr => new
                                {
                                    OrderNumber = cr.RequestId,
                                    Servicename = cr.Service.ServiceName,
                                    Projectdetails = cr.ProjectDetails,
                                    Companyname = cr.CompanyName,
                                    Address = cr.Address,
                                    Requestdate = cr.RequestDate,
                                    Status = cr.Status,
                                    Img = cr.Img,

                                }).ToList() // جلب جميع تفاصيل الطلبات للمستخدم
                            })
                            .ToList();

            return Ok(orders);
        }

        [HttpPost("addassimentforemployee")]
        public IActionResult addassimentforemployee([FromBody] addtaskforemployeeDTO DTO)
        {
            // تحقق من صحة البيانات
            if (DTO.Employees == null || !DTO.Employees.Any())
            {
                return BadRequest("No employees were provided.");
            }

            // إنشاء قائمة لتخزين المهام المضافة
            List<TaskAssignment> addedTasks = new List<TaskAssignment>();

            foreach (var employee in DTO.Employees)
            {
                var addtask = new TaskAssignment
                {
                    //RequestId = DTO.Request.RequestId,
                    EmployeeId = employee.EmployeeId, // استخدام معرف الموظف من القائمة
                    TaskDetails = DTO.Request.ProjectDetails,
                    AssignedDate = DTO.Request.RequestDate,
                    TaskStatus = DTO.TaskStatus,
                };

                _Db.TaskAssignments.Add(addtask);
                addedTasks.Add(addtask); // إضافة المهمة إلى القائمة
            }

            _Db.SaveChanges();

            return Ok(addedTasks); // إرجاع قائمة المهام المضافة
        }


        [HttpGet("getorderbyif{id}")]
        public IActionResult getorderbyid(int id)
        {

            var order = _Db.TaskAssignments.Where(x => x.EmployeeId == id).ToList();
            return Ok(order);
        }


        [HttpPut("editorder/{id}")]
        public IActionResult editorder(int id, editorderDTO DTO)
        {
            var edit = _Db.CustomerRequests.Where(x => x.RequestId == id).FirstOrDefault();
            edit.Status = DTO.Status;
            _Db.CustomerRequests.Update(edit);
            _Db.SaveChanges();

            if (edit.Status!.ToLower() == "completed")
            {
                var payment = _Db.Payments.OrderByDescending(p => p.Id).FirstOrDefault(p => p.UserId == edit.UserId);
                CreatePayment(payment.Id);
            }
            return Ok(edit);
        }

        //[HttpPut("editorderbyemloyee/{id}")]
        //public IActionResult editorderbyemloyee(int id, editorderDTO DTO)
        //{
        //    var edit = _Db.TaskAssignments.Where(x => x.RequestId == id).FirstOrDefault();
        //    edit.TaskStatus = DTO.Status;

        //    _Db.TaskAssignments.Update(edit);
        //    _Db.SaveChanges();

        //    return Ok(edit);
        //}


        private void CreatePayment(int paymentId )
        {
            var storedPayment = _Db.Payments.Find(paymentId);
            var userId = storedPayment.UserId;
            var user = _Db.Users.Find(userId);
            if (string.IsNullOrEmpty(_redirectUrl))
                throw new Exception("The redirect link for the PayPal should be set correctly on the sitting app.");
            var amount = Convert.ToDecimal(storedPayment.TotalAmount) - Convert.ToDecimal(storedPayment.AmountPaid);
            var payment = _paymentService.CreatePayment(paymentId, amount, _redirectUrl);
            var paymentUrl = payment.links.FirstOrDefault(l => l.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase))?.href;
            var messageBody = $@"
            Complete Your Payment
            Dear User,
            Please complete the partial payment using the link below:
            Payment Link: {paymentUrl}
            Amount: {amount} out of {amount * 2}
            Thanks you!";
            // Send the email with the payment URL and PDF attachment if available
            _emailHelper.SendMessage("Admin", user.Email, "Admin Response - Complete Your Payment", messageBody);
        }



        [HttpPost("PostService")]
        public IActionResult PostService([FromForm] orderDTO order12)
        {
            // Check if there is an existing request with the same meeting date
            var existingOrder = _Db.CustomerRequests
                .FirstOrDefault(o => o.MeetingDate == order12.MeetingDate);

            // If the date is already booked, return an error message
            if (existingOrder != null)
            {
                return BadRequest("This meeting date is already booked. Please choose another date.");
            }

            // Create a new request
            var addorder = new CustomerRequest
            {
                UserId = order12.UserId,
                ServiceId = order12.ServiceId,
                ProjectDetails = order12.ProjectDetails,
                CompanyName = order12.CompanyName,
                MeetingDate = order12.MeetingDate,
                Address = order12.Address,
                Status = "Pending",
                RequestDate = DateTime.Now,
            };

            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

            // Ensure the folder exists, if not, create it
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            // Save the image if one was uploaded
            if (order12.Img != null && order12.Img.Length > 0)
            {
                // Generate a unique file name to avoid conflicts
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(order12.Img.FileName);

                // Full file path inside wwwroot
                var filePath = Path.Combine(uploadsFolderPath, fileName);

                // Save the file to the specified folder
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    order12.Img.CopyTo(stream);
                }

                // Save the relative path to the image in the database
                addorder.Img = $"{fileName}";
            }

            // Add the new order to the database
            _Db.Add(addorder);
            _Db.SaveChanges();

            return Ok(addorder);
        }

    }
}
