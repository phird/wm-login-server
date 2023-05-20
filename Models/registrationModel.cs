using System.ComponentModel.DataAnnotations;
using MongoDB.Driver;
namespace wm_api.Models
{
    public class RegistrationModel
    {
        [Required]
        public string firstName {get; set;}

        [Required]
        public string lastName {get; set;}

        [Required]
        public string username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Password and confirm password do not match.")]
        public string confirm { get; set; }
    }

}