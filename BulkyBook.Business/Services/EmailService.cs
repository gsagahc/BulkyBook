using BulkyBook.Business.Services.IServices;
using BulkyBook.Utility;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.Business.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _secretKey;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiKey = _configuration["Mailjet:ApiKey"];
            _secretKey = _configuration["Mailjet:SecretKey"];
            _senderEmail = _configuration["Mailjet:SenderEmail"];
            _senderName = _configuration["Mailjet:SenderName"];
        }
        public async Task<bool> SendEmailAsync(Email emailObj)
        {
            try
            {
                MailjetClient client = new MailjetClient(_apiKey, _secretKey);
                var email = new TransactionalEmailBuilder().WithFrom(new SendContact(_senderEmail, _senderName))
                    .WithTo(new SendContact(emailObj.MailTo)).WithSubject(emailObj.Subject).WithHtmlPart(emailObj.HtmlContent).Build();
                var response = await client.SendTransactionalEmailAsync(email);
                if (response.Messages != null && response.Messages.Length > 0)
                {
                    var message = response.Messages[0];

                    if(message.Status == "success")
                    {
                        return  true;
                    }
                    else
                    {
                        return false;
                    }
                
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SendOrderConfirmationAsync(Email emailObj)
        {
            emailObj.Subject = $"Order Confirmation #{emailObj.OrderId} - BulkyBook";

            // Simple HTML email to demonstrate email functionality
            emailObj.HtmlContent = $@"
                <h1>Thank you for your order!</h1>
                <p>Your order has been placed successfully.</p>
                <hr />
                <p><strong>Order Number:</strong> {emailObj.OrderId}</p>
                <p><strong>Order Date:</strong> {DateTime.Now:MMMM dd, yyyy}</p>
                <p><strong>Total Amount:</strong> {emailObj.OrderTotal:C}</p>
                <hr />
                <p>Thank you for shopping with BulkyBook!</p>
                <p>- The BulkyBook Team</p>";

            return await SendEmailAsync(emailObj);
        }

        public async Task<bool> SendOrderShippedInformationAsync(Email emailObj)
        {
            emailObj.Subject = $"Order Confirmation #{emailObj.OrderId} - BulkyBook";

            // Simple HTML email to demonstrate email functionality
            emailObj.HtmlContent = $@"
                <h1>Order Shipped sucessfully!</h1>
                <p>Your order has been shipped successfully.</p>
                <hr />
                <p><strong>Order Number:</strong> {emailObj.OrderId}</p>
                <p><strong>Order Date:</strong> {DateTime.Now:MMMM dd, yyyy}</p>
                <p><strong>Total Amount:</strong> {emailObj.OrderTotal:C}</p>
                <p><strong>Client address:</strong> {emailObj.UserAddress}</p>
                <p><strong>Client City:</strong> {emailObj.UserCity}</p>
                <p><strong>Client postal code:</strong> {emailObj.UserPostalCode}</p>
              
                <hr />
                <p>Thank you for shopping with BulkyBook!</p>
                <p>- The BulkyBook Team</p>";

            return await SendEmailAsync(emailObj);
        }
    }
}
