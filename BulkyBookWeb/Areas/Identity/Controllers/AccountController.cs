using BulkyBook.Business.Services.IServices;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Identity.Controllers
{
    [Area("Identity")]  
    public class AccountController : Controller
    {
        private readonly IApplicationUserService _applicationUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController( UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
          
            _userManager = userManager;
            _signInManager = signInManager;
        }
       
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM, string? returnUrl = null)
        {
            if (ModelState.IsValid) {
                
                var result = await _signInManager.PasswordSignInAsync(userName:loginVM.Email,
                 password:loginVM.Password, isPersistent: loginVM.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    
                    return RedirectToAction("", "", new { area = "Costumer" });
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(loginVM);
        }


        public IActionResult Register()
        {
           
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Register")]
        public async Task<IActionResult> RegisterPost(RegisterVM registerVM)
        {
            
            if (ModelState.IsValid)
            {
                ApplicationUser userNameExists = await _userManager.FindByEmailAsync(registerVM.Email);
               
                if (userNameExists != null)
                {
                    ModelState.AddModelError("", "User already exists.");
                    return View(registerVM);
                }
                var user = new ApplicationUser
                {
                    UserName = registerVM.Email,
                    Email = registerVM.Email,
                    Name = registerVM.Name,
                    PhoneNumber = registerVM.PhoneNumber,
                    StreetAddress = registerVM.StreetAddress,
                    City = registerVM.City,
                    State = registerVM.State,
                    PostalCode = registerVM.PostalCode
                };
                var result = await _userManager.CreateAsync(user, registerVM.Password);
                if (result.Succeeded)
                {
                    TempData["success"] = "User created successfully";
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("", "", new { area = "Costumer" });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            return View(registerVM);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("", "", new { area = "Costumer" });
        }
    }
}
