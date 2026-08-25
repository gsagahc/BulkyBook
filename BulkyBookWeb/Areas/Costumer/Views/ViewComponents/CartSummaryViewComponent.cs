using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BulkyBook.Business.Services.IServices;
using BulkyBook.Models;
using BulkyBook.DataAccess.Data;
namespace BulkyBookWeb.Areas.Costumer.Views.ViewComponents
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly IShoppingCartService _shoppingCartService;
        public CartSummaryViewComponent(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return View(0);
            }
            var cartCount = await _shoppingCartService.GetShoppingCartCount(userId);
            return View(cartCount);
        }
    }
}
