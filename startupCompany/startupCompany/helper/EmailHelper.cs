using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace startupCompany.helper
{
    public class EmailHelper
    {
        private readonly IConfiguration _configuration;

        public EmailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendMessage(string name, string email, string subject, string message, byte[] attachmentData = null, string attachmentName = null)
        {
            try
            {
                string toEmail = email;
                string smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                string smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                string smtpServer = _configuration["EmailSettings:SmtpServer"];
                int smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);

                // Validate the inputs
                if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
                if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.");
                if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("Message is required.");

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(smtpUsername, name.Trim());
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = subject;
                    mailMessage.Body = message.Trim();
                    mailMessage.IsBodyHtml = false;

                    // Add PDF attachment if provided
                    if (attachmentData != null && !string.IsNullOrEmpty(attachmentName))
                    {
                        using (var memoryStream = new MemoryStream(attachmentData))
                        {
                            mailMessage.Attachments.Add(new Attachment(memoryStream, attachmentName));
                            // Send the email with attachment
                            using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                            {
                                smtpClient.UseDefaultCredentials = false;
                                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                                smtpClient.EnableSsl = true;

                                smtpClient.Send(mailMessage);
                            }
                        }
                    }
                    else
                    {
                        // Send email without attachment
                        using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                        {
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                            smtpClient.EnableSsl = true;

                            smtpClient.Send(mailMessage);
                        }
                    }
                }
            }
            catch (SmtpException smtpEx)
            {
                throw new InvalidOperationException("Error sending email via SMTP", smtpEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while sending the email", ex);
            }
        }
    }
}
