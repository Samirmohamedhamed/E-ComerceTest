using EComerceTestSamir.Models;
using EComerceTestSamir.Models.ViewModels;
using EComerceTestSamir.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;

namespace EComerceTestSamir.Areas.Identity.Controllers
{
    [Area("Identity")]


    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender emailSender;
        private readonly IApplicationUserOTPRepository applicationUserOTPRepository;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager,IEmailSender emailSender
            ,IApplicationUserOTPRepository applicationUserOTPRepository,RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.applicationUserOTPRepository = applicationUserOTPRepository;
            this.roleManager = roleManager;
        }
        public async Task<IActionResult> Rigester()
        {
            if(roleManager.Roles.IsNullOrEmpty())
            {
              await roleManager.CreateAsync(new(SD.SuperAdmin));
              await roleManager.CreateAsync(new(SD.Admin));
              await roleManager.CreateAsync(new(SD.Customer));
              await roleManager.CreateAsync(new(SD.Company));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>Rigester(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);

            }
            ApplicationUser applicationUser = new()
            {
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                Address = registerVM.Address,
            };
            var result =   await userManager.CreateAsync(applicationUser, registerVM.Password);
            if (result.Succeeded)
            {
               await userManager.AddToRoleAsync(applicationUser,SD.Customer);
                string token = await userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                if(token ==null)
                {
                    Console.WriteLine("Token null");
                }
                var comfirmationLink = Url.Action("ConfirmEmail", "Account", new { area = "Identity", applicationUser.Id, token },Request.Scheme);
                if (comfirmationLink == null)
                {
                    Console.WriteLine("comfirmationLink null");
                }
                await emailSender.SendEmailAsync(applicationUser.Email, "Confirmation Email", $"<h1>Confirmation Your Account By Click <a href='{ comfirmationLink }'>Here</a></h1>");
                TempData[key:"Notification"] = "Add Account Successfully,Confirm Your Email";
                return RedirectToAction("Index", "Home", new { area = "Customer"});
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, item.Description);

                }
            }
            return View(registerVM);


        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }
            var applicationUser = await userManager.FindByEmailAsync(loginVM.UserNameOrEmail);
            if(applicationUser == null)
            {
                applicationUser = await userManager.FindByNameAsync(loginVM.UserNameOrEmail);
            }
            if (applicationUser is not null)
            {
                var result = await userManager.CheckPasswordAsync(applicationUser,loginVM.Password);
                if(result)
                {
                   await signInManager.SignInAsync(applicationUser, loginVM.RememberMe);
                    TempData[key: "Notification"] = "Login Successfuly";
                    return RedirectToAction("Index", "Home", new { area = "Customer" });

                }
                ModelState.AddModelError("Password", "Invalid Password");
                return View(loginVM);

            }
            if (applicationUser == null)
            {
                ModelState.AddModelError("UserNameOrEmail", "Invalid User Name Or Email");
                return View(loginVM);
            }
           
            return View();
        }
        public async Task<IActionResult> Logout()
        {
           await signInManager.SignOutAsync();
            TempData[key: "Notification"] = "Logout Successfuly";
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
        public async Task<IActionResult> ConfirmEmailAsync(string id,string token)
        {
           var applicationuser= await userManager.FindByIdAsync(id);
            if (applicationuser is not null)
            {
                var result = await userManager.ConfirmEmailAsync(applicationuser, token);
                if (result.Succeeded)
                {
                    TempData[key: "Notification"] = "Confirm Account Successfully";
                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                }
                else
                {
                   
                        TempData[key: "NotificationError"] = string.Join(",", result.Errors.Select(e => e.Description));

                }
            }
            return BadRequest();
        }
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            if (!ModelState.IsValid)
            {
                return View(resendEmailConfirmationVM);

            }
            var applicationUser = await userManager.FindByEmailAsync(resendEmailConfirmationVM.UserNameOrEmail);
            if (applicationUser == null)
            {
                applicationUser = await userManager.FindByNameAsync(resendEmailConfirmationVM.UserNameOrEmail);
            }
            if (applicationUser is not null)

            {
                if (!applicationUser.EmailConfirmed)
                {
                    string token = await userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                    var comfirmationLink = Url.Action("ConfirmEmail", "Account", new { area = "Identity", applicationUser.Id, token }, Request.Scheme);
                    if (comfirmationLink == null)
                    {
                        Console.WriteLine("comfirmationLink null");
                    }
                    await emailSender.SendEmailAsync(applicationUser.Email, "Resend Email", $"<h1>Confirmation Your Account By Click <a href='{comfirmationLink}'>Here</a></h1>");
                    TempData[key: "Notification"] = "Send Email Successfully";
                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                }
                else
                {
                    TempData[key: "NotificationError"] = "Alredy Confirmation";
                    return RedirectToAction("Index", "Home", new { area = "Customer" });


                }
            }
            ModelState.AddModelError("UserNameOrEmail", "Invalid User Name Or Email");
            return View(resendEmailConfirmationVM);
        }
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {

            if (!ModelState.IsValid)
            {
                return View(forgetPasswordVM);

            }
            var applicationUser = await userManager.FindByEmailAsync(forgetPasswordVM.UserNameOrEmail);
            if (applicationUser == null)
            {
                applicationUser = await userManager.FindByNameAsync(forgetPasswordVM.UserNameOrEmail);
            }
            if (applicationUser is not null)
            {
                string token = await userManager.GeneratePasswordResetTokenAsync(applicationUser);
                var OTPNumber = new Random().Next(1000, 9999);

                //var PasswordResetLink = Url.Action("ResetPassword", "Account", new { area = "Identity", ApplicationUserId = applicationUser.Id, Token = token }, Request.Scheme);
                //await emailSender.SendEmailAsync(applicationUser.Email, "Reset Password", $"<h1>Reset Your Password  Account By Click <a href='{PasswordResetLink}'>Here</a></h1>");
                //return RedirectToAction("Index", "Home", new { area = "Customer" });
                var OTPInDb = applicationUserOTPRepository.Get(e => e.ApplicationUserId == applicationUser.Id).LastOrDefault();
                if(OTPInDb is not null && (DateTime.UtcNow - OTPInDb.ReleaseDate).TotalMinutes >10)
                {
                    await applicationUserOTPRepository.CreateAsync(new()
                    {
                        ApplicationUserId = applicationUser.Id,
                        OTP = OTPNumber,
                        ReleaseDate = DateTime.UtcNow,
                        ExpirationDate = DateTime.UtcNow.AddMinutes(2),

                    });
                    await applicationUserOTPRepository.CommitAsync();
                    await emailSender.SendEmailAsync(applicationUser.Email, "OTP Number", $"<h1>The OTP Number : {OTPNumber}</a></h1>");
                    TempData[key: "Notification"] = "Check Your Email,find OTP Number";
                    TempData[key: "_Validation"] = Guid.NewGuid().ToString();
                    return RedirectToAction("ResetPassword", "Account", new { area = "Identity", ApplicationUserId = applicationUser.Id, Token = token });

                }
                ModelState.AddModelError(string.Empty, "There is Error");
                return View(forgetPasswordVM);


            }
            ModelState.AddModelError("UserNameOrEmail", "Invalid User Name Or Email");
            return View(forgetPasswordVM);
        }
        public IActionResult ResetPassword()
        {
            if (TempData[key: "_Validation"] is not null)
            {
                return View();
            }
          return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            var applicationuser = await userManager.FindByIdAsync(resetPasswordVM.ApplicationUserId);
           
            if (applicationuser is not null)
            {
                var OTPInDb =  applicationUserOTPRepository.Get(e => e.ApplicationUserId == resetPasswordVM.ApplicationUserId).LastOrDefault();

                if (OTPInDb.OTP == resetPasswordVM.OTP && OTPInDb.ExpirationDate<DateTime.UtcNow)
                {
                    var result = await userManager.ResetPasswordAsync(applicationuser, resetPasswordVM.Token, resetPasswordVM.Password);
                    if (result.Succeeded)
                    {
                        TempData[key: "Notification"] = "Confirm Account Successfully";
                        return RedirectToAction("Index", "Home", new { area = "Customer" });
                    }
                    else
                    {

                        TempData[key: "NotificationError"] = string.Join(",", result.Errors.Select(e => e.Description));

                    }
                }
                ModelState.AddModelError("OTP", "Invalid OTP or Expiration");
                return View(resetPasswordVM);

            }
            return BadRequest();
        }
        //ResendEmailConfirmationVM
    }
}
