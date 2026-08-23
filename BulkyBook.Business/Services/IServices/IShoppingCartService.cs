using System;
using System.Collections.Generic;
using System.Text;
using BulkyBook.Models;

namespace BulkyBook.Business.Services.IServices
{
    public interface IShoppingCartService
    {
        Task<int> GetShoppingCartCount(string userId);
        Task<ShoppingCart?> GetCartByIdAsync (int cartId);
        Task<IEnumerable<ShoppingCart>> GetCartByUserIdAsync(string userId);
        Task<ShoppingCart> AddToCartAsync(ShoppingCart cart);
        Task<ShoppingCart> UpdateCartAsync(ShoppingCart cart);
        Task ClearCartAsync(string userId);
        Task <IEnumerable<ShoppingCart>> GetUserCartItensAsync(string userId);
        Task<int> GetCartCountAsync(string userId);

        Task<ShoppingCart?> GetCartItemByIdAsync(int cartId);
        Task IncrementCartItemCountAsync(ShoppingCart cartItem);
        Task DecrementCartItemCountAsync(ShoppingCart cartItem);
        Task RemoveCartItemAsync(ShoppingCart cartItem);

    }
}
