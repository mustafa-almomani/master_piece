namespace startupCompany.DTO
{
    public class getorderDTO
    {

        public int? UserId { get; set; }

        public int? ServiceId { get; set; }

        public string? ProjectDetails { get; set; }

        public IFormFile? Img { get; set; }
        public userorderDTO user { get; set; }

    }
    public class userorderDTO { 
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

}
}
