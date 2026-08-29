using BulkyBook.Business.Services;
using BulkyBook.Business.Services.IServices;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleTypes.RoleAdmin + "," + RoleTypes.RoleEmployee)]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IApplicationUserService _userService;

        public UserController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IApplicationUserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<ActionResult> ChangePassword(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }
        
            AdminChangePasswordVM adminChangePassowordVM = new()
            {
                UserEmail = user.Email,
                UserId = user.Id
                
            };

            return View(adminChangePassowordVM);
        }
        [HttpPost]
        public async Task<ActionResult> ChangePasswordPost(string NewPassword, string UserId)
        {
            ApplicationUser user = await _userService.GetUserByIdAsync(UserId);
             
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }
            string token = await _userService.GerarTokenReset(user);
           
            await _userService.ResetPasswordAsync(user, token, NewPassword);
            TempData["success"] = "Password updated successfully";
            return RedirectToAction("Index");
        }
      

        public ActionResult Create()
        {
            return View();
        }


        public async Task<IActionResult>  RoleManagment(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user != null)
            {
                RoleManagmentVM RoleVM = new()
                {
                    ApplicationUser = user,
                    RoleList = _roleManager.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Name

                    })
                };
                RoleVM.ApplicationUser.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

                return View(RoleVM);
            }
            else
            {
                return Json(new { success = false, message = "User not found" });
            }
            


        }
        [HttpPost]
        public async Task<IActionResult> RoleManagmentPost(RoleManagmentVM roleManagmentVM)
        {
            var user = await _userService.GetUserByIdAsync(roleManagmentVM.ApplicationUser.Id);
            if (user != null)
            {
                string oldRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                if (!(roleManagmentVM.ApplicationUser.Role == oldRole))
                {
                    
                    await _userManager.RemoveFromRoleAsync(user, oldRole);
                    await _userManager.AddToRoleAsync(user, roleManagmentVM.ApplicationUser.Role);
                }
                TempData["success"] = "Role has been updated";
                return RedirectToAction(nameof(Index));

            }
            else
            {
                return Json(new { success = false, message = "User not found" });
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

       [ActionName("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (await _userService.UserHasOrdersAsync(id))
            {
                TempData["error"] = "User cannot be deleted because User has orders.";
                return RedirectToAction("Index");
            }
            var user = await _userService.GetUserByIdAsync(id);
            if (user != null)
            {
                await _userService.DeleteUserAsync(user);
                TempData["success"] = "User deleted successfully";
            }
            else
            {
                TempData["error"] = "User not found";
            }    
            return RedirectToAction("Index");

        }
        #region API CALLS
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();

            foreach (var user in users)
            {
                user.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            }

            return Json(new { data = users });
        }
        [HttpPost]
        public async Task<IActionResult> LockUnlockUser([FromBody] string id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }
            IdentityResult result = await _userService.LockUnlockUserAsync(user);
            return Json(new { success = true, message = result.ToString() });
          
        }
        #endregion
    }
}
