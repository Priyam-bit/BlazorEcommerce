using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEcommerce.Shared
{
    public class UserRegister
    {
        [Required, StringLength(20, ErrorMessage = "Maximum length 20 characters")]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress(ErrorMessage = "Please enter a valid email id")]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 6, 
            ErrorMessage = "Password should have a  minimum length" +
            " of 6 and a maximum length of 100.")]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "The passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
