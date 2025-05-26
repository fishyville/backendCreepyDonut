using Microsoft.EntityFrameworkCore;
using CreepyDonut.Data;
using CreepyDonut.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreepyDonut.DTO;

namespace CreepyDonut.Services
{
    public class CartItemService
    {
        private readonly ApiContext _context;

        public CartItemService(ApiContext context)
        {
            _context = context;
        }

        // Fix for CS1061: Replace 'Id' with 'CartItemId' in the projection of GetCartItemsAsync method.  
        public async Task<List<CartItemDto>> GetCartItemsAsync(int cartId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .Include(ci => ci.Product) // assuming navigation property to Product  
                .ToListAsync();

            return cartItems.Select(ci => new CartItemDto
            {
                Id = ci.CartItemId, // Fixed: Use 'CartItemId' instead of 'Id'  
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                Quantity = ci.Quantity,
                Price = ci.Product.Price,
                ImageUrl = ci.Product.ImageUrl,
                CategoryId = ci.Product.CategoryId
            }).ToList();
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
