using BulkyBook.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.Business.Services.IServices
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(Email email);
        Task<bool> SendOrderConfirmationAsync(Email email);
        Task<bool> SendOrderShippedInformationAsync(Email email);
    }
}
