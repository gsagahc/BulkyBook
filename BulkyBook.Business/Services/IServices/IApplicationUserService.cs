using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
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

        
    }
}
