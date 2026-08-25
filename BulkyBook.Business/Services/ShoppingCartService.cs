using BulkyBook.Business.Services.IServices;
using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Business.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartService(ApplicationDbContext context)
        {
            _context = context;
        }
        public Task<int> GetShoppingCartCount(string userId)
        {
           return _context.ShoppingCarts.Where(c => c.UserId == userId).SumAsync(c => c.Count);
        }

        public async Task<ShoppingCart?> GetCartByIdAsync(int cartId)
        {
            return await _context.ShoppingCarts.Include(u => u.Product).FirstOrDefaultAsync(u => u.Id == cartId);
        }

        public async Task<IEnumerable<ShoppingCart>> GetCartByUserIdAsync(string userId)
        {
            return await _context.ShoppingCarts.Where(c => c.UserId == userId).Include(u => u.Product).ToListAsync();
        }
        

        public Task<ShoppingCart> AddToCartAsync(ShoppingCart cart)
        {
           var existingCartItem = _context.ShoppingCarts.FirstOrDefault(c => c.UserId == cart.UserId && c.ProductId == cart.ProductId);
            if (existingCartItem != null)
            {
                existingCartItem.Count += cart.Count;
                _context.ShoppingCarts.Update(existingCartItem);
            }
            else
            {
                _context.ShoppingCarts.Add(cart);
            }
            _context.SaveChanges();
            return Task.FromResult(cart);
        }

        public Task<ShoppingCart> UpdateCartAsync(ShoppingCart cart)
        {
            if(cart.Count <= 0)
            {
                _context.ShoppingCarts.Remove(cart);
            }
            else
            {
                _context.ShoppingCarts.Update(cart);
            }
            _context.SaveChanges();
            return Task.FromResult(cart);
        }

        public async Task ClearCartAsync(string userId)
        {
            var cartItens = await _context.ShoppingCarts.Include(u => u.Product).Where(u => u.UserId == userId).ToListAsync();
            if (cartItens.Any())
            {
                _context.ShoppingCarts.RemoveRange(cartItens);
                await _context.SaveChangesAsync();
            }
           
           
        }

        public async Task<IEnumerable<ShoppingCart>> GetUserCartItensAsync(string userId)
        {
            return await _context.ShoppingCarts.Include(u => u.Product).Where(u => u.UserId == userId).ToListAsync();
        }

        public async Task<int> GetCartCountAsync(string userId)
        {
            return await _context.ShoppingCarts.Where(u => u.UserId == userId).SumAsync(u => u.Count);
        
        }

        public Task<ShoppingCart?> GetCartItemByIdAsync(int cartId)
        {
           return _context.ShoppingCarts.Include(u => u.Product).FirstOrDefaultAsync(u => u.Id == cartId);
        }

        public Task IncrementCartItemCountAsync(ShoppingCart cartItem)
        {
            if (cartItem.Count >= 1000)
            {
                cartItem.Count = 1000;
            }
            else
            {
                cartItem.Count++;
            }
            return UpdateCartAsync(cartItem);
        }

        public Task DecrementCartItemCountAsync(ShoppingCart cartItem)
        {
            cartItem.Count--;
            return UpdateCartAsync(cartItem);
        }

        public async Task RemoveCartItemAsync(ShoppingCart cartItem)
        {
            _context.ShoppingCarts.Remove(cartItem);
            await _context.SaveChangesAsync();
            
        }
    }
}
