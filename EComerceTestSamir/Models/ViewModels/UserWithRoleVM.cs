using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EComerceTestSamir.Models.ViewModels
{
    public class UserWithRoleVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        [ValidateNever]
        public List<IdentityRole>  identityRoles { get; set; }
    }
}
