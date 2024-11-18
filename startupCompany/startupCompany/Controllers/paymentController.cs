using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPal.Api;
using startupCompany.DTO;
using startupCompany.helper;
using startupCompany.Models;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly StripePaymentService _stripePaymentService;
    private readonly MyDbContext _context;
    private readonly EmailHelper _emailHelper;
    private readonly string _redirectUrl;
    private readonly IConfiguration _configuration;
    private readonly PayPalPaymentService _paymentService;



    public PaymentController(StripePaymentService stripePaymentService, MyDbContext context, EmailHelper emailHelper, IConfiguration configuration, PayPalPaymentService paymentService)
    {
        _stripePaymentService = stripePaymentService;
        _context = context;
        _emailHelper = emailHelper;
        _configuration = configuration;
        _paymentService = paymentService;
        _redirectUrl = configuration["PayPal:RedirectUrl"];



    }

    // POST api/payment/initiate
    //[HttpPost("initiate")]
    //public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentDto paymentDto)
    //{
    //    try
    //    {
    //        var paymentRecord = new Payment
    //        {
    //            UserId = paymentDto.UserId,
    //            TotalAmount = paymentDto.TotalAmount,
    //            AmountPaid = (decimal.Parse(paymentDto.TotalAmount) ).ToString(),   
    //            RequestId = paymentDto.RequestId,
    //            PaymentStatus = "Partially Paid"
    //        };

    //        _context.Payments.Add(paymentRecord);
    //        await _context.SaveChangesAsync();

    //        // Initiate Stripe checkout for the first 50%
    //        var sessionId = await _stripePaymentService.CreateCheckoutSessionAsync(
    //            "Product Payment - 50%",
    //            (long)(decimal.Parse(paymentDto.TotalAmount) * 100), // Convert to cents for Stripe
    //            paymentDto.SuccessUrl,
    //            paymentDto.CancelUrl
    //        );

    //        return Ok(new { SessionId = sessionId });
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { message = ex.Message });
    //    }
    //}

    // POST api/payment/complete/{paymentId}
    //[HttpPost("complete/{paymentId}")]
    //public async Task<IActionResult> CompletePayment(int paymentId)
    //{
    //    var payment = await _context.Payments.FindAsync(paymentId);
    //    if (payment == null)
    //        return NotFound(new { message = "Payment not found." });

    //    try
    //    {
    //        payment.AmountPaid = payment.TotalAmount;
    //        payment.PaymentStatus = "Completed";

    //        await _context.SaveChangesAsync();

    //        return Ok(new { message = "Payment completed successfully." });
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { message = ex.Message });
    //    }




    //}

    //[HttpPost("adminResponse")]
    //public IActionResult AdminResponse([FromForm] EmailPartially adminres, IFormFile pdfFile)
    //{
    //    try
    //    {
    //        // Construct the payment URL with query parameters
    //        var paymentUrl = $"http://127.0.0.1:5500/paymentuser.html?userId={adminres.UserId}&orderId={adminres.OredrID}&amount={adminres.amount}";

    //        // Compose the email message
    //        var messageBody = $@"
    //    Complete Your Payment
    //    Dear User,
    //    Please complete your payment by clicking the link below:
    //    Payment Page: {paymentUrl}
    //    Amount: {adminres.amount}";

    //        // Read the uploaded PDF file if it's provided
    //        byte[] pdfData = null;
    //        string attachmentName = pdfFile?.FileName;

    //        if (pdfFile != null && pdfFile.Length > 0)
    //        {
    //            using (var memoryStream = new MemoryStream())
    //            {
    //                pdfFile.CopyTo(memoryStream);
    //                pdfData = memoryStream.ToArray();
    //            }
    //        }

    //        // Send the email with the payment URL and PDF attachment if available
    //        _emailHelper.SendMessage("Admin", adminres.Email, "Admin Response - Complete Your Payment", messageBody, pdfData, attachmentName);

    //        return Ok("Email sent successfully with PDF attachment.");
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine("Error: " + ex.Message);
    //        return StatusCode(500, "An error occurred while sending the email.");
    //    }
    //}


    [HttpGet("orderinfo/{id}")]

    public IActionResult orderinfo(int id)
    {
        var order = _context.CustomerRequests.Where(x => x.RequestId == id).Select(y => y.Service.ServiceName).ToList();
        return Ok(order);
    }



    [HttpGet("getallpayment")]
    public IActionResult getallpayment()
    {
        var payments = _context.Payments
            .Include(p => p.User)
            .Select(p => new
            {
                Id = p.Id,
                UserId = p.UserId,
                TotalAmount = p.TotalAmount,
                AmountPaid = p.AmountPaid,
                RequestId = p.RequestId,
                PaymentStatus = p.PaymentStatus,
                User = p.User == null ? null : new
                {
                    UserId = p.User.UserId,
                    FirstName = p.User.FirstName,
                    LastName = p.User.LastName
                },
             
            })
            .ToList();

        return Ok(payments);
    }























    [HttpPost("adminResponse")]
    public IActionResult AdminResponse([FromForm] EmailPartially adminres, IFormFile pdfFile)
    {
        if (string.IsNullOrEmpty(_redirectUrl))
            throw new Exception("The redirect link for the paypal should be set correctly on the sitting app.");



        var totalPrice = Convert.ToDecimal(adminres.amount);
        var payment = _paymentService.CreatePayment(_redirectUrl ?? " ", totalPrice/2, null, adminres.UserId,totalPrice,adminres.OredrID, "Partially");
        var paymentUrl = payment.links.FirstOrDefault(l => l.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase))?.href;
        try
        {
            // Construct the payment URL with query parameters
            //var paymentUrl = $"http://127.0.0.1:5500/paymentuser.html?userId={adminres.UserId}&orderId={adminres.OredrID}&amount={adminres.amount}";

            // Compose the email message
            var messageBody = $@"
        Complete Your Payment
        Dear User,
        Please complete the partial payment using the link below:
        Payment Link: {paymentUrl}
        Amount: {totalPrice / 2} out of {totalPrice}
        You will be required to complete the rest of the amount after completing your order.
";

            // Read the uploaded PDF file if it's provided
            byte[] pdfData = null;
            string attachmentName = pdfFile?.FileName;

            if (pdfFile != null && pdfFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    pdfFile.CopyTo(memoryStream);
                    pdfData = memoryStream.ToArray();
                }
            }

            // Send the email with the payment URL and PDF attachment if available
            _emailHelper.SendMessage("Admin", adminres.Email, "Admin Response - Complete Your Payment", messageBody, pdfData, attachmentName);

            return Ok("Email sent successfully with PDF attachment.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return StatusCode(500, "An error occurred while sending the email.");
        }
    }


    [HttpGet("completePayment")]
    public IActionResult ExecutePayment(string paymentId, string PayerID, string token, int storedPaymentId, decimal totalPaid, decimal amount)
    {
        var paymentRecord = _context.Payments.Find(storedPaymentId);
        paymentRecord.AmountPaid = Convert.ToString(Convert.ToDecimal(paymentRecord.AmountPaid) + amount);
        paymentRecord.PaymentStatus = "Completed";
        _context.Payments.Update(paymentRecord);
        _context.SaveChanges();

        var executedPayment = _paymentService.ExecutePayment(paymentId, PayerID);

        string script = "<script>window.close();</script>";
        return Content(script, "text/html");
    }

    [HttpGet("success")]
    public IActionResult ExecutePayment(string paymentId, string PayerID, string token, int userId, decimal totalPaid, decimal totalAmount, int requestId, string paymentStatus)
    {


        var paymentRecord = new startupCompany.Models.Payment
        {
            UserId = userId,
            TotalAmount = Convert.ToString(totalAmount),
            AmountPaid = Convert.ToString(totalPaid),
            RequestId = requestId,
            PaymentStatus = paymentStatus
        };

        _context.Payments.Add(paymentRecord);
        _context.SaveChanges();




        var executedPayment = _paymentService.ExecutePayment(paymentId, PayerID);

        string script = "<script>window.close();</script>";
        return Content(script, "text/html");
    }



}
