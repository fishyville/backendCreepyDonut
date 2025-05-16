using Microsoft.EntityFrameworkCore;
using CreepyDonut.Data;
using CreepyDonut.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CreepyDonut.Services
{
    public class CartService
    {
        private readonly ApiContext _context;

        public CartService(ApiContext context)
        {
            _context = context;
        }

        // GET CART BY USER ID
        public async Task<Cart> GetCartByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)        // Include the CartItems list
                .ThenInclude(ci => ci.Product)    // Include the associated Product for each CartItem
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        // CREATE CART FOR USER (KALO USER GA PUNYA CART)
        public async Task<Cart> CreateCartAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new ArgumentException("Invalid userId");

            var cart = new Cart { UserId = userId, User = user };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        // ADD PRODUCT TO CART
        public async Task<bool> AddProductToCartAsync(int cartId, int productId, int quantity)
        {
            var cart = await _context.Carts.FindAsync(cartId);
            var product = await _context.Products.FindAsync(productId);

            if (cart == null || product == null)
                return false;

            var cartItem = new CartItem
            {
                CartId = cartId,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = product.Price
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        // UPDATE CART ITEM QUANTITY
        public async Task<bool> UpdateCartItemQuantityAsync(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
                return false;

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
            return true;
        }

        // REMOVE PRODUCT FROM CART
        public async Task<bool> RemoveProductFromCartAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
                return false;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        // CLEAR CART
        public async Task<bool> ClearCartAsync(int cartId)
        {
            var cartItems = _context.CartItems.Where(ci => ci.CartId == cartId);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
