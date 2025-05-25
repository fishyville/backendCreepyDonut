using CreepyDonut.Data;
using CreepyDonut.DTO;
using CreepyDonut.Models;
using Microsoft.EntityFrameworkCore;

namespace CreepyDonut.Services
{
    public class OrderServices
    {
        private readonly ApiContext _context;

        public OrderServices(ApiContext context)
        {
            _context = context;
        }

        public async Task<List<OrderResponse>> GetAllAsync()
        {
            return await _context.Orders
                .Select(o => new OrderResponse
                {
                    OrderId = o.OrderId,
                    UserId = o.UserId,
                    CartId = o.CartId,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status,
                    PaymentMethod = o.PaymentMethod,
                    ShippingAddress = o.ShippingAddress,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<OrderResponse?> GetByIdAsync(int id)
        {
            var o = await _context.Orders.FindAsync(id);
            if (o == null) return null;

            return new OrderResponse
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                CartId = o.CartId,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod,
                ShippingAddress = o.ShippingAddress,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            };
        }

        public async Task<OrderResponse> CreateAsync(OrderRequest req)
        {
            var user = await _context.Users
                .Include(u => u.Cart)
                .FirstOrDefaultAsync(u => u.UserId == req.UserId);
            if (user == null) throw new Exception("User not found");

            var cart = await _context.Carts
                .Include(c => c.Order)
                .FirstOrDefaultAsync(c => c.CartId == req.CartId && c.UserId == user.UserId);
            if (cart == null) throw new Exception("Cart not found");

            // Check if this cart is already used for an order
            if (cart.Order != null)
                throw new Exception("This cart has already been used for an order.");

            var o = new Order
            {
                UserId = req.UserId,
                User = user,
                CartId = req.CartId,
                Cart = cart,
                TotalPrice = req.TotalPrice,
                Status = req.Status,
                PaymentMethod = req.PaymentMethod,
                ShippingAddress = req.ShippingAddress
            };

            _context.Orders.Add(o);
            await _context.SaveChangesAsync();

            // Detach the old cart from the user (break the one-to-one relationship)
            user.Cart = null;
            await _context.SaveChangesAsync();

            // Now create a new cart for the user
            var newCart = new Cart { UserId = user.UserId, User = user };
            _context.Carts.Add(newCart);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(o.OrderId) ?? throw new Exception("Error creating order");
        }

        public async Task<bool> UpdateAsync(int id, OrderRequest req)
        {
            var o = await _context.Orders.FindAsync(id);
            if (o == null) return false;

            o.UserId = req.UserId;
            o.CartId = req.CartId;
            o.TotalPrice = req.TotalPrice;
            o.Status = req.Status;
            o.PaymentMethod = req.PaymentMethod;
            o.ShippingAddress = req.ShippingAddress;
            o.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var o = await _context.Orders.FindAsync(id);
            if (o == null) return false;

            _context.Orders.Remove(o);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.Status = newStatus;
            order.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<OrderResponse>> GetOrderResponsesByUserIdAsync(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .ToListAsync();

            return orders.Select(o => new OrderResponse
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                CartId = o.CartId,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod ?? string.Empty,
                ShippingAddress = o.ShippingAddress,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            }).ToList();
        }
    }
}

