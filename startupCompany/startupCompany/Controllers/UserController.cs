using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using startupCompany.DTO;
using startupCompany.Models;
using System.Data;

namespace startupCompany.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _Db;



        public UserController(MyDbContext db)
        {
            _Db = db;

        }
        [HttpPost]
        public IActionResult regester([FromForm] regesterDTO dto)
        {
            if (dto.password != dto.confairmpassword)
            {
                return BadRequest("Passwords do not match");
            }
            byte[] hash;
            byte[] salt;
            passwordHasherMethod.createPasswordHash(dto.password, out hash, out salt);

            var newuser = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = hash,
                Passwordsult = salt

            };
            _Db.Users.Add(newuser);
            _Db.SaveChanges();

            return Ok();
        }
        [HttpPost("LOGIN")]
        public IActionResult login(LoginDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid request data.");
            }

            var employee = _Db.Employees.FirstOrDefault(x => x.Email == dto.Email);
            if (employee != null)
            {
                if (employee.Password == dto.password)
                {
                    return Ok(employee); 
                }
                else
                {
                    return Unauthorized("Invalid password for employee.");                 }
            }

            var user = _Db.Users.FirstOrDefault(x => x.Email == dto.Email);
            if (user == null)
            {
                return NotFound("User not found.");  
            }

            if (!passwordHasherMethod.VerifyPasswordHash(dto.password, user.PasswordHash, user.Passwordsult))
            {
                return Unauthorized("Invalid username or password."); 
            }

            return Ok(user); 
        }



        [HttpPost("GoogleSignUp")]
        public IActionResult GoogleSignUp([FromBody] SignUpByGoogleDto byGoogleDto)
        {
            if (byGoogleDto == null || string.IsNullOrEmpty(byGoogleDto.Email))
            {
                return BadRequest();
            }
            if (_Db.Users.Any(u => u.Email == byGoogleDto.Email)) { return BadRequest("this email is already in the database"); }
            var user = new User
            {
                FirstName = byGoogleDto.FirstName,
                LastName = byGoogleDto.LastName,
                Email = byGoogleDto.Email,
                PhoneNumber = "*******",

            };
            byte[] hash;
            byte[] salt;
            passwordHasherMethod.createPasswordHash(byGoogleDto.Password, out hash, out salt);
            user.PasswordHash = hash;
            user.Passwordsult = salt;
            _Db.Users.Add(user);
            _Db.SaveChanges();
            return Ok();
        }
        [HttpPost("addemployee")]
        public IActionResult addemployee([FromForm] AddEmployeesDTO EmployeesDTO)
        {
            var Employee = new Employee
            {
                FirstName = EmployeesDTO.FirstName,
                LastName = EmployeesDTO.LastName,
                Email = EmployeesDTO.Email,
                PhoneNumber = EmployeesDTO.PhoneNumber,
                JoinDate = EmployeesDTO.JoinDate,
                Status = EmployeesDTO.Status,
                JobTitle = EmployeesDTO.JobTitle,
                Role = EmployeesDTO.Role,
                Password= EmployeesDTO.Password,
            };
            _Db.Employees.Add(Employee);
            _Db.SaveChanges();
            return Ok();
        }

        [HttpGet("getallemployee")]
        public IActionResult employee()
        {
            var employee = _Db.Employees.ToList();
            return Ok(employee);
        }
        [HttpDelete("deleteemployee{id}")]
        public IActionResult deleteEmployee(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var employee = _Db.Employees.FirstOrDefault(c => c.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            _Db.Employees.Remove(employee);
            _Db.SaveChanges();
            return NoContent();
        }


        [HttpGet("getalladmin")]
        public IActionResult admin()
        {
            var admin = _Db.Employees.Where(x => x.Role == "Admin");
            return Ok(admin);
        }


        [HttpPut("{id}")]
        public IActionResult productDTOput(int id, [FromForm] editemployeeDTO obj)
        {
            var edit = _Db.Employees.Find(id);
            edit.Email = obj.Email ?? edit.Email;
            edit.Role = obj.Role ?? edit.Role;
            edit.Status = obj.Status ?? edit.Status;
            edit.PhoneNumber = obj.PhoneNumber ?? edit.PhoneNumber;
            edit.PerformanceReport = obj.PerformanceReport ?? edit.PerformanceReport;
            edit.JobTitle = obj.JobTitle ?? edit.JobTitle;




            _Db.Update(edit);
            _Db.SaveChanges();
            return Ok();
        }

        [HttpGet("getalluserd")]
        public IActionResult getallusers()
        {
            var users = _Db.Users.Select(x => new usersDTO
            {            
                UserId=x.UserId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
            });
            return Ok(users);

        }




        [HttpPut("edit/user/{id}")]
        public IActionResult useredit(int id, [FromForm] edituserDTO obj)
        {
            var edit = _Db.Users.Find(id);
            //edit.UserId=obj.UserId;
            edit.FirstName = obj.FirstName??edit.FirstName;
            edit.LastName = obj.LastName??edit.LastName;
            edit.Email = obj.Email ?? edit.Email;
            edit.PhoneNumber = obj.PhoneNumber ?? edit.PhoneNumber;




            _Db.Update(edit);
            _Db.SaveChanges();
            return Ok();
        }


        [HttpGet("getusersbyid/{id}")]
        public IActionResult getusersid(int id )
        {
            var user = _Db.Users.Find(id);
            return Ok(user);
        }

        [HttpGet("getemployeebyid/{id}")]
        public IActionResult getemployeeid(int id)
        {
            var employee = _Db.Employees.Find(id);
            return Ok(employee);
        }



        [HttpGet("getmeetingbyid{id}")]
        public IActionResult getmeetingbyid(int id)
        {
            var meeting =_Db.TeamMeetings.Where(x=>x.EmployeeId == id).FirstOrDefault();

            return Ok(meeting);
        }





        /////////////Testimonials API ////////////

        [HttpPost("AddTestimonial/{id}")]
        public IActionResult Addtestimonial(int id, [FromBody] AddtestimonialDTO addtestimonialDTO)
        {
            if (id == null || id == 0)
            {
                return BadRequest("The id is null or 0 here");
            }

            var user = _Db.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var addtestimonial = new Testimonial
            {
                UserId = id,
                Firstname = user.FirstName,
                Lastname = user.LastName,
                Email = user.Email,
                TheTestimonials = addtestimonialDTO.TheTestimonials
            };
            _Db.Testimonials.Add(addtestimonial);
            _Db.SaveChanges();
            return Ok();
        }
        [HttpGet("GetAllAcceptedTestimonial")]
        public IActionResult GetAllAcceptedTestimonial()
        {
            var testmonials = _Db.Testimonials.Where(a => a.Isaccepted == true).ToList();
            var lesttestimonial = new List<GetAcceptTestimonialDTO>();
            foreach (var item in testmonials)
            {
                var accepttestimonial = new GetAcceptTestimonialDTO
                {
                    username = item.Firstname + " "+item.Lastname,
                   
                    TheTestimonials  = item.TheTestimonials,
                    Email=item.Email,
                };
                lesttestimonial.Add(accepttestimonial);
            }

            return Ok(lesttestimonial);

        }



        ////////////////////////////////testimonial///////////
        [HttpGet("GetAllTestimonials")]
        public IActionResult GetAllTestimonials()
        {
            var testimonials = _Db.Testimonials.OrderBy(m => m.Isaccepted).ToList();
            var lestTestimonials = new List<GetAllTestimonialsDTO>();

            foreach (var item in testimonials)
            {
                var testimonial = new GetAllTestimonialsDTO
                {
                    Id = item.Id,
                   username  = item.Firstname+" "+item.Lastname,
                    Email = item.Email,
                    TheTestimonials = item.TheTestimonials,
                    Isaccepted = item.Isaccepted
                };
                lestTestimonials.Add(testimonial);
            }
            return Ok(lestTestimonials);
        }

        [HttpDelete("DeleteTestimonial/{id}")]
        public IActionResult DeleteTestimonial(int id)
        {
            if (id <= 0)
            {
                return BadRequest("You can not use 0 or negative value for id");
            }

            var testmonial = _Db.Testimonials.FirstOrDefault(u => u.Id == id);
            if (testmonial == null)
            {
                return NotFound();
            }
            _Db.Testimonials.Remove(testmonial);
            _Db.SaveChanges();
            return Ok();
        }
        [HttpPut("AcceptTestimonial/{id}")]
        public IActionResult AcceptTestimonial(int id)
        {
            if (id <= 0)
            {
                return BadRequest("You can not use 0 or negative value for id");
            }
            var testimonial = _Db.Testimonials.FirstOrDefault(u => u.Id == id);
            if (testimonial == null)
            {
                return NotFound();
            }
            testimonial.Isaccepted = true;
            _Db.Testimonials.Update(testimonial);
            _Db.SaveChanges();
            return Ok();
        }

        [HttpGet("getorderbyuser{id}")]
        public IActionResult getuserrequest ( int id)
        {
            var userorder = _Db.CustomerRequests.Where(x => x.UserId == id);
            return Ok(userorder);
        }

        [HttpGet("getpaymentbyuser{id}")]
        public IActionResult getpaymentbyuser(int id)
        {
            var userorder = _Db.Payments.Where(x => x.UserId == id);
            return Ok(userorder);
        }




    }



}
