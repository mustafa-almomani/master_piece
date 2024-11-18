namespace startupCompany.DTO
{
    public class InitiatePaymentDto
    {
        public int UserId { get; set; }
        public string TotalAmount { get; set; }
        public int RequestId { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }
    }

}
