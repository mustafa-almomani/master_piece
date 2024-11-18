using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using startupCompany.DTO;
using startupCompany.Models;

namespace startupCompany.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class taskesController : ControllerBase
    {
        private readonly MyDbContext _Db;
            public taskesController(MyDbContext Db)
        {
            _Db = Db;
        }

        [HttpPost("addtask")]
        public IActionResult addtask([FromForm] taskesDTO DTO)
        {
            var task = new TaskAssignment
            {
                TaskDetails = DTO.TaskDetails,
                AssignedDate = DTO.AssignedDate,
                DueDate = DTO.DueDate,
                EmployeeId = DTO.EmployeeId,
                TaskStatus = DTO.TaskStatus,

            };
            _Db.TaskAssignments.Add(task);
            _Db.SaveChanges();
            return Ok(task);
        }


        [HttpGet("getalltaskforadmin")]
        public IActionResult getalltask()
        {
            var alltask=_Db.TaskAssignments.ToList();   
            return Ok(alltask);
        }

        [HttpGet("getemployees")]
        public IActionResult GetEmployees()
        {
            var employees = _Db.Employees.Select(e => new
            {
                e.EmployeeId, 
                name=e.FirstName+" "+e.LastName  
            }).ToList();

            return Ok(employees);
        }



        //////////////////////////


        [HttpGet("getorderbyif{id}")]
        public IActionResult getorderbyid(int id)
        {

            var order = _Db.TaskAssignments.Where(x => x.EmployeeId == id).ToList();
            return Ok(order);
        }


        [HttpPut("edittask/{id}")]
        public IActionResult editorder(int id, edittaskDTO DTO)
        {
            var edit = _Db.TaskAssignments.Where(x => x.TaskId == id).FirstOrDefault();
            edit.TaskStatus = DTO.TaskStatus;

            _Db.TaskAssignments.Update(edit);
            _Db.SaveChanges();

            return Ok(edit);
        }

        [HttpGet("getemployeesperformance")]
        public IActionResult GetEmployeesPerformance()
        {
            // جلب جميع الموظفين مع عدد المهام التي حالتها "Completed" وتقييم الأداء
            var result = _Db.Employees
                .Select(e => new
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeName = e.FirstName + " " + e.LastName,
                    Email=e.Email,
                    role=e.Role,
                    CompletedTasks = _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed"),
                    PerformanceRating = _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed") > 10
                                        ? "Excellent"
                                        : _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed") > 5
                                        ? "Very Good"
                                        : _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed") > 2
                                        ? "Good"
                                        : "Needs Improvement",
                    // إضافة مزيد من التفاصيل التحليلية
                    PerformanceReport = new
                    {
                        // تحليل بناءً على نسبة إنجاز المهام
                        TasksRange = _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed") > 10
                                     ? "High Performer"
                                     : _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed") > 5
                                     ? "Above Average"
                                     : _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed") > 2
                                     ? "Average Performer"
                                     : "Low Performer",

                        // إضافة تقرير عام حول الأداء
                        GeneralReport = _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed") > 10
                                        ? "The employee consistently exceeds expectations by completing a high volume of tasks."
                                        : _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed") > 5
                                        ? "The employee is performing above average and consistently delivers tasks on time."
                                        : _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed") > 2
                                        ? "The employee meets expectations and completes tasks in a timely manner."
                                        : "The employee needs to improve task completion and overall performance."
                    }
                })
                .ToList();

            return Ok(result);
        }



        [HttpGet("getemployeereport{id}")]
        public IActionResult GetEmployeesPerformance(int id)
        {
            // جلب جميع الموظفين مع عدد المهام التي حالتها "Completed" وتقييم الأداء
            var result = _Db.Employees.Where(x=>x.EmployeeId==id)
                .Select(e => new
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeName = e.FirstName + " " + e.LastName,
                    Email = e.Email,
                    role = e.Role,
                    CompletedTasks = _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed"),
                    PerformanceRating = _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed") > 10
                                        ? "Excellent"
                                        : _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed") > 5
                                        ? "Very Good"
                                        : _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed") > 2
                                        ? "Good"
                                        : "Needs Improvement",
                    // إضافة مزيد من التفاصيل التحليلية
                    PerformanceReport = new
                    {
                        // تحليل بناءً على نسبة إنجاز المهام
                        TasksRange = _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed") > 10
                                     ? "High Performer"
                                     : _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed") > 5
                                     ? "Above Average"
                                     : _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed") > 2
                                     ? "Average Performer"
                                     : "Low Performer",

                        // إضافة تقرير عام حول الأداء
                        GeneralReport = _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed") > 10
                                        ? "The employee consistently exceeds expectations by completing a high volume of tasks."
                                        : _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed") > 5
                                        ? "The employee is performing above average and consistently delivers tasks on time."
                                        : _Db.TaskAssignments.Count(t => t.EmployeeId == e.EmployeeId && t.TaskStatus == "Completed") > 2
                                        ? "The employee meets expectations and completes tasks in a timely manner."
                                        : "The employee needs to improve task completion and overall performance."
                    }
                })
                .ToList();

            return Ok(result);
        }


        [HttpDelete("DeleteTask/{id}")]
        public IActionResult DeleteTask(int id)
        {
            if (id <= 0)
            {
                return BadRequest("You can not use 0 or negative value for id");
            }

            var task = _Db.TaskAssignments.FirstOrDefault(u => u.TaskId== id);
            if (task == null)
            {
                return NotFound();
            }
            _Db.TaskAssignments.Remove(task);
            _Db.SaveChanges();
            return Ok();
        }


    }
}
