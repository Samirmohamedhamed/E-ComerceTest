using System.ComponentModel.DataAnnotations;

namespace EComerceTestSamir.Models.ViewModels
{
    public class ResendEmailConfirmationVM
    {
        [Required]
        public string UserNameOrEmail { get; set; } = null!;
     
    }
}
