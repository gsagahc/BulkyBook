using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.Business.Services.IServices
{
    public interface IOrderService
    {
        Task <OrderHeader> CreateOrderAsync(OrderHeader orderHeader);
        Task<OrderHeader?> GetOrderByIdAsync(int id,bool includeUser=false, bool includeDetails=false);
        Task<IEnumerable<OrderHeader?>> GetAllOrdersAsync(string? userId=null,string? status=null, bool includeUser = false, bool includeDetails = false);
        Task <string> CreateStripeCheckoutSessionAsync(OrderHeader orderHeader, List<ShoppingCart> cartItems, string domain);
        Task  UpdateOrderHeaderAsync(OrderHeader orderHeader);
        Task UpdateOrderStausAsync(int id,string status, string? carrier=null,string? trakingNumber=null);
    }
}
