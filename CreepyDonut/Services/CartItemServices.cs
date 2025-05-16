using Microsoft.EntityFrameworkCore;
using CreepyDonut.Data;
using CreepyDonut.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CreepyDonut.Services
{
    public class CartItemService
    {
        private readonly ApiContext _context;

        public CartItemService(ApiContext context)
        {
            _context = context;
        }

        // GET CART ITEMS BY CART ID
        public async Task<List<CartItem>> GetCartItemsAsync(int cartId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product) 
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();
        }

        // ADD ITEM TO CART
        public async Task<bool> AddCartItemAsync(int cartId, int productId, int quantity)
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
        public async Task<bool> UpdateCartItemAsync(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
                return false;

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
            return true;
        }

        // REMOVE CART ITEM
        public async Task<bool> RemoveCartItemAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
                return false;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
