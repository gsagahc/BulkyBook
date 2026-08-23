using BulkyBook.Business.Services.IServices;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Costumer.Controllers
{
    [Area("Costumer")]
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;

        public HomeController(IProductService productService, IShoppingCartService shoppingCartService)
        {
            _productService = productService;
            _shoppingCartService = shoppingCartService;
        }
        public async Task<IActionResult> Index()
        {
            
            var products = await _productService.GetAllProductsAsync(includeCategory: true);
            return View(products);
        }
        public async Task<IActionResult> Product()
        {
            var products = await _productService.GetAllProductsAsync(includeCategory: true);
            return View("Products", products);
        }
        public async Task<IActionResult> Details(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId, includeCategory: true);
            if (product == null)
            {
                return NotFound();
            }
            ShoppingCart cart = new()
            {
                ProductId = product.Id,
                Product = product,
                Count = 1
            };
               
            return View(cart );

        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Details(ShoppingCart cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            cart.UserId = userId;
            await _shoppingCartService.AddToCartAsync(cart);
            var count = await _shoppingCartService.GetCartCountAsync(userId);
            TempData["success"] = "Item added to cart";
            return RedirectToAction("Details", new { productId = cart.ProductId });

        }

        public IActionResult Privacy()
        {
            return View();
        }

      
    }
}
