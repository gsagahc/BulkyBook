using BulkyBook.Business.Services.IServices;
using BulkyBook.Data;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.Business.Services
{
    public class CategoryService : ICategoryService
    {
        public readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetAllcategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        } 

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            // placeholder: return null until real implementation is added
            return await _context.Categories.FindAsync(id);
        }

       

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            // placeholder: echo back the provided category until real implementation is added
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            // placeholder: no-op until real implementation is added
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            // placeholder: no-op until real implementation is added
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
