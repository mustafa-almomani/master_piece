using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace startupCompany.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContacttController : ControllerBase
    {

        private readonly EmailService _emailService;

        public ContacttController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("sendemail")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("Email and message cannot be empty");
            }

            await _emailService.SendEmailAsync(request.Email, "New Message", request.Message);
            return Ok("Email sent successfully");
        }

    }
    public class EmailRequest
    {
        public string Email { get; set; }
        public string Message { get; set; }
    }
}
