using BulkyBook.Business.Services.IServices;
using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks; // added

namespace BulkyBook.Business.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly ApplicationDbContext _context;
     
        public ApplicationUserService(ApplicationDbContext context)
        {
            _context = context;
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

       
    }
}
