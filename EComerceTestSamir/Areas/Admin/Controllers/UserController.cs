using EComerceTestSamir.Models;
using EComerceTestSamir.Models.ViewModels;
using EComerceTestSamir.Repositories.IRepositories;
using EComerceTestSamir.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EComerceTestSamir.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IApplicationUserRepository applicationUserRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRoleRepository roleRepository;
        private readonly IEmailSender emailSender;

        public UserController(IApplicationUserRepository applicationUserRepository, UserManager<ApplicationUser> userManager, IRoleRepository roleRepository,IEmailSender emailSender)
        {
            this.applicationUserRepository = applicationUserRepository;
            this.userManager = userManager;
            this.roleRepository = roleRepository;
            this.emailSender = emailSender;
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = roleRepository.Get();
          
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(AdminCreateUserVM adminCreateUserVM)
        {
            if(!ModelState.IsValid)
            {
                return View(adminCreateUserVM);
            }
            var user = new ApplicationUser()
            {
                UserName = adminCreateUserVM.UserName,
                Email = adminCreateUserVM.Email,
                Address = adminCreateUserVM.Address,

            };
            var result = await userManager.CreateAsync(user, adminCreateUserVM.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, adminCreateUserVM.RoleOfUser);
                string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
               
                var comfirmationLink = Url.Action("ConfirmEmail", "Account", new { area = "Identity", user.Id, token }, Request.Scheme);
                await emailSender.SendEmailAsync(user.Email, "Confirmation Email", $"<h1>Confirmation Your Account By Click <a href='{comfirmationLink}'>Here</a></h1>");
                TempData[key: "Notification"] = "Add Account Successfully,Confirm Your Email";
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, item.Description);

                }
            }
            return View(adminCreateUserVM);
        }
        public async Task<IActionResult> Index()
        {
            var users = applicationUserRepository.Get();
            Dictionary<ApplicationUser, string> UsersRoles = new();

            foreach (var item in users )
            {
                var roles = await userManager.GetRolesAsync(item);

                UsersRoles.Add(item, string.Join(",", roles));
            }

            return View(UsersRoles);
        }
        public async Task<IActionResult> ChangeRole(string id)
        {
            var role = roleRepository.Get();
            var user = await userManager.FindByIdAsync(id);
            if (user is not null)
            {
                return View(new UserWithRoleVM()
                {
                    ApplicationUser = user,
                    identityRoles = role.ToList()

                });
            }
            return BadRequest();
        }  
        [HttpPost]
        public async Task<IActionResult> ChangeRole(UserWithRoleVM userWithRoleVM,string role)
        {
            ModelState.Remove("userWithRoleVM.identityRoles");
            if(!ModelState.IsValid)
            {
                return View(userWithRoleVM);
            }
            var user = await userManager.FindByIdAsync(userWithRoleVM.ApplicationUser.Id);
            if(user is not null)
            {
               var roles =  await userManager.GetRolesAsync(user);
                await userManager.RemoveFromRolesAsync(user, roles);
                await userManager.AddToRoleAsync(user,role);

            }
          
            return RedirectToAction("Index", "User",new{area = "Admin" });
        }
        public async Task<IActionResult> LockUnLock(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user is not null)
            {
                user.LockoutEnabled = !user.LockoutEnabled;
                if(!user.LockoutEnabled)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddDays(1);

                }
                else
                {
                    user.LockoutEnd = null;
                }
            }
            await userManager.UpdateAsync(user);
            return RedirectToAction("Index", "User", new {area = "Admin"});
        }

        
    }
}

