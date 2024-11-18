using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using startupCompany.DTO;
using startupCompany.helper;
using startupCompany.Models;

namespace startupCompany.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class contactController : ControllerBase
    {

        private readonly MyDbContext _Db;
        private readonly EmailHelper _emailHelper;
        public contactController(MyDbContext db, EmailHelper emailHelper)
        {
            _Db = db;
            _emailHelper = emailHelper;

        }

        [HttpPost("newmassege")]
        public IActionResult addcontact([FromForm] contactDTO dto)
        {
            var messege = new ContactMessage
            {
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                MessageDate = dto.MessageDate = DateTime.Now,
                MessageText = dto.MessageText,
            };

            _Db.ContactMessages.Add(messege);
            _Db.SaveChanges();
            return Ok();
        }


        [HttpGet("getallcontact")]
        public IActionResult getallcontact()
        {
            var messege = _Db.ContactMessages.ToList();
            return Ok(messege);
        }
        [HttpPost("adminRespone")]
        public async Task<IActionResult> adminRespone([FromForm]AdminresDto adminres)
        {
            try {
                _emailHelper.SendMessage("to user", adminres.Email, "admin respone", adminres.messege);
            } catch (Exception ex) {
                Console.WriteLine("error");
            }

            return Ok();
        }

        [HttpDelete("deleteMessage/{id}")]
        public IActionResult DeleteMessage(int id)
        {
            var message = _Db.ContactMessages.Find(id);
            if (message == null)
            {
                return NotFound();
            }

            _Db.ContactMessages.Remove(message);
            _Db.SaveChanges();

            return Ok();
        }

    }
}
