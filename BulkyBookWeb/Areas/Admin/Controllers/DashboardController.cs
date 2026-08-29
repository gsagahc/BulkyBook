using BulkyBook.Business.Services;
using BulkyBook.Business.Services.IServices;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Employee")]
    public class DashboardController : Controller
    {
        private readonly  IApplicationUserService _userService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        public DashboardController(IProductService productService, IApplicationUserService userService, IOrderService orderService)
        {
            _productService = productService;
            _userService = userService;
            _orderService = orderService;

        }
        public async Task<IActionResult> Index()
        {
            int OrderCount = await _orderService.GetOrderCountAsync();
            DashboardVM dashBoardVM = new()
            {
                TotalOrders = OrderCount
            };
            
            return View(dashBoardVM);
        }
    }
}
