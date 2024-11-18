﻿using PayPal.Api;
namespace startupCompany.helper
{

    public class PayPalPaymentService
    {
        private readonly string clientId;
        private readonly string clientSecret;

        public PayPalPaymentService(IConfiguration config)
        {
            this.clientId = config["PayPal:ClientId"];
            this.clientSecret = config["PayPal:ClientSecret"];
        }

        private APIContext GetAPIContext()
        {
            var oauthToken = new OAuthTokenCredential(clientId, clientSecret).GetAccessToken();
            return new APIContext(oauthToken);
        }

        public Payment CreatePayment(string redirectUrl, decimal totalPaid, string? message, int userId, decimal totalAmount, int requestId, string paymentStatus)
        {
            var apiContext = GetAPIContext();

            // Define payment details
            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        amount = new Amount
                        {
                            currency = "USD",
                            total = $"{totalPaid}" // amount to charge
                        },
                        description = message?? "Product description"
                    }
                },
                redirect_urls = new RedirectUrls
                {
                    cancel_url = $"{redirectUrl}/cancel",
                    return_url = $"{redirectUrl}/success?userId={userId}&totalPaid={totalPaid}&totalAmount={totalAmount}&requestId={requestId}&paymentStatus={paymentStatus}"
                }
            };

            // Create payment using PayPal API
            return payment.Create(apiContext);
        }

        public Payment CreatePayment(int paymentId, decimal amount, string redirectUrl)
        {
            var apiContext = GetAPIContext();

            // Define payment details
            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        amount = new Amount
                        {
                            currency = "USD",
                            total = $"{amount}" // amount to charge
                        },
                        description =  "Complete your payment"
                    }
                },
                redirect_urls = new RedirectUrls
                {
                    cancel_url = $"{redirectUrl}/cancel",
                    return_url = $"{redirectUrl}/completePayment?amount={amount}&storedPaymentId={paymentId}"
                }
            };

            // Create payment using PayPal API
            return payment.Create(apiContext);
        }

        public Payment ExecutePayment(string paymentId, string payerId)
        {
            var apiContext = GetAPIContext();
            var paymentExecution = new PaymentExecution { payer_id = payerId };
            var payment = new Payment { id = paymentId };

            // Execute payment
            return payment.Execute(apiContext, paymentExecution);
        }
    }
}