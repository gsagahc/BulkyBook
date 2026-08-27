using BulkyBook.Business.Services.IServices;
using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;
using BulkyBook.Utility;

namespace BulkyBook.Business.Services
{
    public class OrderService : IOrderService
    {
        public readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OrderHeader> CreateOrderAsync(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Add(orderHeader);
            await _context.SaveChangesAsync();
            return orderHeader;
        }

        public Task<string> CreateStripeCheckoutSessionAsync(OrderHeader orderHeader, List<ShoppingCart> cartItems, string domain)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<OrderHeader?>> GetAllOrdersAsync(string? userId = null, string? status = null, bool includeUser = false, bool includeDetails = false)
        {
            var query = _context.OrderHeaders.AsQueryable();
            if (includeUser)
            {
                query = query.Include(o => o.ApplicationUser);
            }
            if (includeDetails)
            {
                query = query.Include(o => o.OrderDetails).ThenInclude(od => od.Product);
            }
            if(!string.IsNullOrEmpty(userId))
            {
                query = query.Where(o => o.ApplicationUserId == userId);
            }
            if (!string.IsNullOrEmpty(status))
            {
                if (status != "all")
                {
                    query = query.Where(o => o.OrderStatus.ToLower() == status.ToLower());
                }
                else
                {
                    query = query.Where(o => o.OrderStatus != "");
                }
            }
            return await query.ToListAsync();
        }

        public Task<OrderHeader?> GetOrderByIdAsync(int id, bool includeUser = false, bool includeDetails = false)
        {
            var query = _context.OrderHeaders.AsQueryable();
            if (includeUser)
            {
                query = query.Include(o => o.ApplicationUser);
            }
            if (includeDetails)
            {
                query = query.Include(o => o.OrderDetails).ThenInclude(od => od.Product);
            }
            return query.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateOrderHeaderAsync(OrderHeader orderHeader)
        {
           
            var orderFromDb = await _context.OrderHeaders
                .FirstOrDefaultAsync(o => o.Id == orderHeader.Id);

            if (orderFromDb != null)
            {
               
                orderFromDb.Name = orderHeader.Name;
                orderFromDb.PhoneNumber = orderHeader.PhoneNumber;
                orderFromDb.StreetAddress = orderHeader.StreetAddress;
                orderFromDb.City = orderHeader.City;
                orderFromDb.State = orderHeader.State;
                orderFromDb.PostalCode = orderHeader.PostalCode;

               if (!string.IsNullOrEmpty(orderHeader.Carrier) && orderFromDb.OrderStatus == Status.StatusShipped)
                {
                    orderFromDb.Carrier = orderHeader.Carrier;
                }
                if(!string.IsNullOrEmpty(orderHeader.TrackingNumber) && orderFromDb.OrderStatus==Status.StatusShipped)
                {
                    orderFromDb.TrackingNumber = orderHeader.TrackingNumber;
                }                        
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Order not found");
            }

        }

        public async Task UpdateOrderStausAsync(int id, string status, string? carrier = null, string? trakingNumber = null)
        {
            var orderFromDb = await _context.OrderHeaders.FindAsync(id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = status;
                if (orderFromDb.OrderStatus == Status.StatusShipped)
                {
                    orderFromDb.ShippingDate = DateTime.Now;
                    if (!string.IsNullOrEmpty(carrier))
                    {
                        orderFromDb.Carrier = carrier;
                    }
                    if (!string.IsNullOrEmpty(trakingNumber))
                    {
                        orderFromDb.TrackingNumber = trakingNumber;
                    }
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"Order {id} not found");
            }
        }
    }
}
