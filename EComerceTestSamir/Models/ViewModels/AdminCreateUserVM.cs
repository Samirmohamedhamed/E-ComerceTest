using System.ComponentModel.DataAnnotations;

namespace EComerceTestSamir.Models.ViewModels
{
    public class AdminCreateUserVM
    {
            [Required]
            public string UserName { get; set; } = null!;
            [Required]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; } = null!;
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = null!;
            [Required]
            [DataType(DataType.Password)]
            [Compare(nameof(Password))]
            public string ConfirmPassword { get; set; } = null!;
            public string? Address { get; set; }
            public String RoleOfUser { get; set; } 

        }
}
