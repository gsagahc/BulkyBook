using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.Utility
{
    public class Email
    {
        public string? MailTo { get; set; }
        public string? Subject { get; set; }
        public string? HtmlContent { get; set; }
        public string? UserAddress { get; set; }
        public string? UserCity { get; set; }
        public string? UserPostalCode { get; set; }
        public string? OrderCarrier { get; set; }

        public string? OrderTrakingNumber { get; set; }
        public int OrderId { get; set; }

        public decimal OrderTotal { get; set; }
    }
}
