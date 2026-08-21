using BulkyBook.Business.Services.IServices;
using BulkyBook.Models;
using BulkyBook.Models.Enums;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;

namespace BulkyBookWeb.Areas.Identity.Controllers
{
    [Area("Identity")]  
    public class AccountController : Controller
    {
        private readonly IApplicationUserService _applicationUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(IApplicationUserService applicationUserService, UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _applicationUserService = applicationUserService;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
       
        public IActionResult Login(string? returnUrl=null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(returnUrl);
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
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                        return RedirectToAction("Index", "Home", new { area = "Costumer" });
                   
                     
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(loginVM);
        }


        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!_roleManager.RoleExistsAsync(RoleTypes.RoleAdmin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(RoleTypes.RoleAdmin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(RoleTypes.RoleCostumer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(RoleTypes.RoleEmployee)).GetAwaiter().GetResult();
            }
            var model = new RegisterVM();
            if (User.IsInRole(RoleTypes.RoleAdmin))
            {
                model.RoleList =
                    [
                     new SelectListItem{Text=RoleTypes.RoleCostumer, Value=RoleTypes.RoleCostumer},
                     new SelectListItem{Text=RoleTypes.RoleAdmin, Value=RoleTypes.RoleAdmin},
                     new SelectListItem{Text=RoleTypes.RoleEmployee, Value=RoleTypes.RoleEmployee},
                ];
            }
            else
            {
                model.RoleList = new List<SelectListItem> { new SelectListItem { Text = RoleTypes.RoleCostumer, Value = RoleTypes.RoleCostumer } };
            }
            ViewBag.Paises = new SelectList(Enum.GetValues(typeof(CountryEnum)));
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Register")]
        public async Task<IActionResult> RegisterPost(RegisterVM registerVM, string? returnUrl = null)
        {
            
            if (ModelState.IsValid)
            {
                ApplicationUser userNameExists = await _userManager.FindByEmailAsync(registerVM.Email);
               
                if (userNameExists != null)
                {
                    ModelState.AddModelError("", "User already exists.");
                    ViewBag.Paises = new SelectList(Enum.GetValues(typeof(CountryEnum)));
                    return View(registerVM);
                }
                //var user = new ApplicationUser();
                var user = new ApplicationUser
                {
                    Name = registerVM.Name,
                    Email = registerVM.Email,
                    UserName = registerVM.Email,
                    StreetAddress = registerVM.StreetAddress,
                    City = registerVM.City,
                    State = registerVM.State,
                    PostalCode = registerVM.PostalCode,
                    Country = registerVM.Country,
                    PhoneNumber = registerVM.PhoneNumber,
                    Role = registerVM.Role
                };

                var result = await _userManager.CreateAsync(user, registerVM.Password);    


                if (result.Succeeded)
                {
                    if(!string.IsNullOrEmpty(registerVM.Role))
                    {
                        await _userManager.AddToRoleAsync(user, registerVM.Role);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, RoleTypes.RoleCostumer);
                    }
                    TempData["success"] = "User created successfully";
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("", "", new { area = "Costumer" });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            ViewBag.Paises = new SelectList(Enum.GetValues(typeof(CountryEnum)));
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
