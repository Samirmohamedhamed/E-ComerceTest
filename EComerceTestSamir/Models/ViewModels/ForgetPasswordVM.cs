using System.ComponentModel.DataAnnotations;

namespace EComerceTestSamir.Models.ViewModels
{
    public class ForgetPasswordVM
    {
        [Required]
        public string UserNameOrEmail { get; set; } = null!;
    }
}
