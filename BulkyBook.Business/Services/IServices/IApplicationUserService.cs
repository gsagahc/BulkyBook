using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.Business.Services.IServices
{
    public interface IApplicationUserService
    {
        Task<ApplicationUser> CreateApplicationUserAsync(ApplicationUser user);
        Task<ApplicationUser> AddRegister(RegisterVM registerVM);
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
        Task<string> GerarTokenReset(ApplicationUser user);
        Task <bool> IsUserLockedAsync(ApplicationUser user);
        Task<IdentityResult> LockUnlockUserAsync(ApplicationUser user);
        Task<bool> UserHasOrdersAsync(string id);
        Task DeleteUserAsync(ApplicationUser user);


    }
}
