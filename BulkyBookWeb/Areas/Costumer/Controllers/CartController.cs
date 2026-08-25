using BulkyBook.Business.Services;
using BulkyBook.Business.Services.IServices;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Costumer.Controllers
{
    [Area("Costumer")]
    [Authorize]
    public class CartController : Controller
    {
       
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IOrderService _orderService;

        public CartController(IOrderService orderService, IShoppingCartService shoppingCartService, IApplicationUserService applicationUserService)
        {
            _orderService = orderService;
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
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
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
        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost (ShoppingCartVM shoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var cartItems = await _shoppingCartService.GetUserCartItensAsync(userId);
            shoppingCartVM.ShoppingCartList = cartItems;
            shoppingCartVM.OrderHeader.OrderDate = DateTime.UtcNow;
            shoppingCartVM.OrderHeader.ApplicationUserId = userId;

            foreach (var item in cartItems)
            {
                var count = Request.Form[$"cartItem_{item.Id}"];
                if (int.TryParse(count, out int newCount))
                {
                    await UpdateCartAsync(item.Id, newCount);
                }
            }
            shoppingCartVM.OrderHeader.OrderStatus = Status.StatusApproved;
            shoppingCartVM.OrderHeader.OrderDetails = shoppingCartVM.ShoppingCartList.Select(cart => new OrderDetails
            {
                ProductId = cart.ProductId,
                Price = cart.Price,
                Count = cart.Count,
            }).ToList();

            await _orderService.CreateOrderAsync(shoppingCartVM.OrderHeader);
            return RedirectToAction("OrderConfirmation", new { id = shoppingCartVM.OrderHeader.Id });
           

        }
        public async Task<IActionResult> OrderConfirmation(int id)
        {
           return View(id);
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
        public async Task<IActionResult> UpdateCartAsync(int cartId, int count)
        {
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);
            if (cart == null)
            {
                return NotFound();
            }
            if (count <= 1)
            {
                cart.Count = 0;
                await _shoppingCartService.UpdateCartAsync(cart);
            }
            else
            {
                if (count >= 1000)
                {
                    cart.Count = 1000;
                }
                else
                {
                    cart.Count = count;
                }

               
            }
            await _shoppingCartService.UpdateCartAsync(cart);
            return RedirectToAction(nameof(Index));

        }
    }
}
