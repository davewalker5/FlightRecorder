using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Mvc.Models
{
    public class LoginViewModel
    {
        [DisplayName("Username")]
        [Required(ErrorMessage = "You must enter a user name")]
        public string UserName { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "You must enter a password")]
        public string Password { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// A parameterless default constructor is required for model binding
        /// </summary>
        public LoginViewModel()
        {
        }
    }
}
