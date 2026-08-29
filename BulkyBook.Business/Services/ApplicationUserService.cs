using BulkyBook.Business.Services.IServices;
using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks; // added

namespace BulkyBook.Business.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
     
        public ApplicationUserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Task<ApplicationUser> AddRegister(RegisterVM registerVM)
        {
           var user = new ApplicationUser
           {
               UserName = registerVM.Email,
               Email = registerVM.Email,
               Name = registerVM.Name,
               StreetAddress = registerVM.StreetAddress,
               City = registerVM.City,
               State = registerVM.State,
               PostalCode = registerVM.PostalCode,
               Country = registerVM.Country,
               PhoneNumber = registerVM.PhoneNumber
           };
            return Task.FromResult(user);
        }

        public async Task<ApplicationUser> CreateApplicationUserAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<string> GerarTokenReset(ApplicationUser user)
        {
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public Task<bool> IsUserLockedAsync(ApplicationUser user)
        {
            return _userManager.IsLockedOutAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
         
             IdentityResult resultado = await _userManager.ResetPasswordAsync(user, token, newPassword);

            return resultado;
        }

        public async Task<IdentityResult> LockUnlockUserAsync(ApplicationUser user)
        {
            bool isLocked = await IsUserLockedAsync(user);
            if (isLocked)
            {
                IdentityResult resultado = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                return resultado;
            } 
            else
            {
                IdentityResult resultado = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(1000));
                return resultado;
            }
            
        }

        public async Task<bool> UserHasOrdersAsync(string id)
        {
            return await _context.OrderHeaders.AnyAsync(p => p.ApplicationUserId == id);
        }

        public async Task DeleteUserAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new KeyNotFoundException($"User not found.");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

        }
    }
}
