using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BulkyBook.Models.ViewModels
{
    public class RegisterVM
    {

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required]  
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; } = null;
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Street Address")]
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        [Display(Name = "Phone number")]
        public string phoneNumber { get; set; } = string.Empty;
    }
}
