using System.ComponentModel.DataAnnotations;

namespace EComerceTestSamir.Models.ViewModels
{
    public class LoginVM
    {
        [Required]
        public string UserNameOrEmail { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
       
    }
}
