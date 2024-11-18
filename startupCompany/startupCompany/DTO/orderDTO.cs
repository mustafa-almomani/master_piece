using startupCompany.Models;

namespace startupCompany.DTO
{
    public class orderDTO
    {
       

        public int? UserId { get; set; }

        public int? ServiceId { get; set; }

        public string? ProjectDetails { get; set; }

        public IFormFile? Img { get; set; }
        public string? CompanyName { get; set; }
        public string? Address { get; set; }
        public DateTime? MeetingDate { get; set; }




    }

}
