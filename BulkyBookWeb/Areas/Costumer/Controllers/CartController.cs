using BulkyBook.Business.Services;
using BulkyBook.Business.Services.IServices;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Costumer.Controllers
{
    [Area("Costumer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IApplicationUserService _applicationUserService;

        public CartController(IProductService productService, IShoppingCartService shoppingCartService, IApplicationUserService applicationUserService)
        {
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _applicationUserService = applicationUserService;
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var cartItems = await _shoppingCartService.GetUserCartItensAsync(userId);
            var user = await _applicationUserService.GetUserByIdAsync(userId);
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM
            {
                ShoppingCartList = cartItems,
                OrderHeader = new()
            };
            shoppingCartVM.OrderHeader.ApplicationUser = user;
            shoppingCartVM.OrderHeader.ApplicationUserId = userId;
            shoppingCartVM.OrderHeader.Name = user.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
            shoppingCartVM.OrderHeader.StreetAddress = user.StreetAddress;
            shoppingCartVM.OrderHeader.City = user.City;
            shoppingCartVM.OrderHeader.State = user.State;
            shoppingCartVM.OrderHeader.PostalCode = user.PostalCode;
            foreach (var item in shoppingCartVM.ShoppingCartList)
            {
                item.Product = await _productService.GetProductByIdAsync(item.ProductId);
                shoppingCartVM.OrderHeader.OrderTotal += (item.Product.Price * item.Count);
            }
            return View(shoppingCartVM);
        }
        public async Task<IActionResult> Plus(int cartId)
        {
            var cartItem = await _shoppingCartService.GetCartItemByIdAsync(cartId);
            if (cartItem != null)
            {
                await _shoppingCartService.IncrementCartItemCountAsync(cartItem);
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Minus(int cartId)
        {
            var cartItem = await _shoppingCartService.GetCartItemByIdAsync(cartId);
            if (cartItem != null)
            {
                await _shoppingCartService.DecrementCartItemCountAsync(cartItem);
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int cartId)
        {
            var cartItem = await _shoppingCartService.GetCartItemByIdAsync(cartId);
            if (cartItem != null)
            {
                await _shoppingCartService.RemoveCartItemAsync(cartItem);
            }
            return RedirectToAction(nameof(Index));
        }



    }
}
