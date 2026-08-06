using BulkyBook.Business.Services.IServices;
using BulkyBook.Data;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.Business.Services
{
    public class ProductService : IProductService
    {
        public readonly ApplicationDbContext _context;

        public ProductService (ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync(bool includeCategory=false)
        {
            if (includeCategory)
            {
                return await _context.Products.Include(p => p.Category).ToListAsync();
            }
            else
            {
                return await _context.Products.ToListAsync();
            }
        } 

        public async Task<Product?> GetProductByIdAsync(int id)
        {
           
            return await _context.Products.FindAsync(id);
        }

       

        public async Task<Product> CreateProductAsync(Product product)
        {
           
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

      
        public async Task DeleteProductAsync(int id)
        {
           
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsProductTitleUniqueAsync(string title, int? id = null)
        {
            if (id.HasValue)
            {
                return !await _context.Products.AnyAsync(p => p.Title.ToLower() == title.ToLower() && p.Id != id.Value);
            }
            else
            {
                return !await _context.Products.AnyAsync(p => p.Title.ToLower() == title.ToLower());
            }
        }
    }
}
