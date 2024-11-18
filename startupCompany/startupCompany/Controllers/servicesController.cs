using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using startupCompany.DTO;
using startupCompany.Models;

namespace startupCompany.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class servicesController : ControllerBase
    {
        private readonly MyDbContext _Db;



        public servicesController(MyDbContext db)
        {
            _Db = db;

        }
        [HttpGet("getallservices")]
        public IActionResult getallservices()
        {
            var services = _Db.Services.ToList();
            return Ok(services);
        }


   [HttpPost]
 public IActionResult addservies([FromForm] servicesDTO product)
 {
     var service = new Service();
     service.ServiceName = product.ServiceName;
     service.ServiceDescription = product.ServiceDescription;

     var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

     // التأكد من وجود المجلد، وإذا لم يكن موجودًا يتم إنشاؤه
     if (!Directory.Exists(uploadsFolderPath))
     {
         Directory.CreateDirectory(uploadsFolderPath);
     }

     // تحديث الصورة إذا تم رفعها
     if (product.Img != null && product.Img.Length > 0)
     {
         // اسم الملف الفريد لتجنب التعارض
         var fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.Img.FileName);

         // المسار الكامل للملف داخل wwwroot
         var filePath = Path.Combine(uploadsFolderPath, fileName);

         // حفظ الملف في المجلد المحدد
         using (var stream = new FileStream(filePath, FileMode.Create))
         {
             product.Img.CopyTo(stream);
         }

         // حفظ المسار النسبي للصورة في قاعدة البيانات
         service.Img = $"{fileName}";
     }
     _Db.Services.Add(service);
     _Db.SaveChanges();
     return Ok(service);
 }

        [HttpPut("editservices/{id}")]
        public IActionResult updatecategory(int id, [FromForm] servicesDTO obj)
        {

            var service = _Db.Services.Find(id);
            var uploadImageFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadImageFolder))
            {
                Directory.CreateDirectory(uploadImageFolder);
            }
            var imageFile = Path.Combine(uploadImageFolder, obj.Img.FileName);
            using (var stream = new FileStream(imageFile, FileMode.Create))
            {
                obj.Img.CopyToAsync(stream);
            }

            service.ServiceName = obj.ServiceName ?? service.ServiceName;
            service.ServiceDescription = obj.ServiceDescription ?? service.ServiceDescription;
            service.Img = obj.Img.FileName ?? service.Img;

            _Db.Services.Update(service);
            _Db.SaveChanges();

            return Ok();
        }



        [HttpGet("getservicesbyid/{id}")]
        public IActionResult getusersid(int id)
        {
            var user = _Db.Services.Find(id);



            return Ok(user);
        }




        [HttpDelete("deleteservicesid")]
        public IActionResult deleteservices(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var Servic = _Db.Services.FirstOrDefault(c => c.ServiceId == id);
            if (Servic == null)
            {
                return NotFound();
            }

            _Db.Services.Remove(Servic);
            _Db.SaveChanges();
            return NoContent();
        }
        [HttpGet("userInfo")]
        public IActionResult userInfo(int userid)
        {
            var userinfo = _Db.Users.Select(a => new
            {
                a.FirstName,
                a.LastName,
                a.Email,
                a.PhoneNumber,
                a.UserId

            }).FirstOrDefault(a => a.UserId == userid);

            return Ok(userinfo);
        }
        [HttpPost("PostService")]
        public IActionResult PostService([FromForm] orderDTO order12)
        {
            var addorder = new CustomerRequest
            {
                UserId = order12.UserId,
                ServiceId = order12.ServiceId,
                ProjectDetails = order12.ProjectDetails,
                CompanyName = order12.CompanyName,
              
                Address = order12.Address,
                Status = "Pending",
                RequestDate = DateTime.Now,
            };
            if (order12.Img != null && order12.Img.Length > 0)
            {
                var uploadImageFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

                if (!Directory.Exists(uploadImageFolder))
                {
                    Directory.CreateDirectory(uploadImageFolder);
                }


                var fileName = Path.GetFileName(order12.Img.FileName);


                var imageFile = Path.Combine(uploadImageFolder, fileName);

                using (var stream = new FileStream(imageFile, FileMode.Create))
                {
                    order12.Img.CopyTo(stream);
                }

                addorder.Img = fileName;
            }

            _Db.Add(addorder);
            _Db.SaveChanges();

            return Ok(addorder);
        }


        [HttpGet("getallorder")]
        public IActionResult GetAllOrders()
        {
            // استخدام Include لجلب بيانات المستخدم والخدمة المرتبطة بالطلب
            var orders = _Db.CustomerRequests
                            .Include(cr => cr.User)    // جلب بيانات المستخدم المرتبط
                            .Include(cr => cr.Service) // جلب بيانات الخدمة المرتبطة
                            .AsEnumerable()            // لتحويل الاستعلام إلى ذاكرة لاستخدام الفهرس
                            .Select((cr, index) => new
                            {userid=cr.UserId,
                                OrderNumber = index + 1,  // إضافة العداد
                                Requestid = cr.RequestId,
                                Firstname = cr.User.FirstName,
                                Lastname = cr.User.LastName,
                                Email = cr.User.Email,
                                Phonnumber = cr.User.PhoneNumber,
                                Servicename = cr.Service.ServiceName,
                                Projectdetails = cr.ProjectDetails,
                                Companyname = cr.CompanyName,
                                Address = cr.Address,
                                Requestdate = cr.RequestDate,
                                Status = cr.Status,
                                Img = cr.Img,
                                meetingdate=cr.MeetingDate,
                            })
                            .ToList();

            return Ok(orders);
        }

        [HttpGet("getjoptitleforemployee")]
        public IActionResult GetJobTitleForEmployee(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                return BadRequest("Service name is required.");
            }

            // جلب الموظفين بناءً على اسم الخدمة المُدخل
            var result = from emp in _Db.Employees
                         join serv in _Db.Services
                         on emp.JobTitle equals serv.ServiceName
                         where serv.ServiceName == serviceName // قارن اسم الخدمة المُدخل مع ServiceName
                         select new
                         {
                             EmployeeName = emp.FirstName + " " + emp.LastName,
                             JobTitle = emp.JobTitle,
                             ServiceName = serv.ServiceName,
                             employeeid=emp.EmployeeId,
                         };

            if (!result.Any())
            {
                return NotFound("No matching employees found.");
            }

            return Ok(result);
        }


        [HttpGet("getcountorderdone")]
        public IActionResult getcountorderdone()
        {
            var getcountorderdone =_Db.CustomerRequests.Where(x=>x.Status== "Completed").Count();
            var getcountinprogres = _Db.Users.Count();
            var employee = _Db.Employees.Count();
            var result = new
            {
                Completed = getcountorderdone,
                InProgress = getcountinprogres,
                employee=employee
            };

            return Ok(result);
        }



    }
}
