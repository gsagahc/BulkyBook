using BulkyBook.Business.Services.IServices;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleTypes.RoleAdmin)]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        [BindProperty]
        public OrderHeader OrderHeader { get; set; }
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }            
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int orderId)
        {
            OrderHeader = await _orderService.GetOrderByIdAsync(orderId, includeDetails:true, includeUser:true);
            return View(OrderHeader);
        }
        #region API CALLS
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(string status)
        { string? userId = null;
            if (!User.IsInRole(RoleTypes.RoleAdmin) && !User.IsInRole(RoleTypes.RoleEmployee))
            {
                var claimsIdentity = (System.Security.Claims.ClaimsIdentity)User.Identity;
                userId = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

            }
            var orders = await _orderService.GetAllOrdersAsync(userId, status);
            return Json(new { data = orders });
        }
        #endregion
    }
}
