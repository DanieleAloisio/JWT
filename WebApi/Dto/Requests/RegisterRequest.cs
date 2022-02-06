using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto.Requests
{
    public class RegisterRequest
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "First Name required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name required")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "Birth Date required")]
        //public DateTime BirthDate { get; set; }

        //public string? Sex { get; set; }
    }
}
