using Microsoft.EntityFrameworkCore;
using CreepyDonut.Data;
using CreepyDonut.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreepyDonut.DTO;

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

        public async Task<CartDto> GetCartDtoByUserIdAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return null;

            var cartDto = new CartDto
            {
                Id = cart.CartId,
                Items = cart.CartItems.Select(ci => new CartItemDto
                {
                    Id = ci.CartItemId,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.Name,
                    Quantity = ci.Quantity,
                    Price = ci.Product?.Price
                }).ToList()
            };

            return cartDto;
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

            if (product.Quantity < quantity)
                throw new InvalidOperationException("Not enough stock available.");

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price
                };
                _context.CartItems.Add(cartItem);
            }

            // Reduce the product's available quantity
            product.Quantity -= quantity;
            Console.WriteLine($"Product {product.Name} new stock: {product.Quantity}");
            await _context.SaveChangesAsync();
            return true;
        }


        // UPDATE CART ITEM QUANTITY
        public async Task<bool> UpdateCartItemQuantityAsync(int cartId, int productId, int quantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartId, productId);
            if (cartItem == null)
                return false;

            // Get the product from the database
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return false;

            // Calculate the difference in quantity
            int quantityDifference = quantity - cartItem.Quantity;

            // Update the cart item quantity
            cartItem.Quantity = quantity;

            // Adjust the product quantity based on the difference
            if (quantityDifference > 0)
            {
                if (product.Quantity < quantityDifference)
                    throw new InvalidOperationException("Not enough stock available.");

                // Reduce the product stock
                product.Quantity -= quantityDifference;
            }
            else if (quantityDifference < 0)
            {
                // Increase the product stock
                product.Quantity += Math.Abs(quantityDifference);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // REMOVE PRODUCT FROM CART
        public async Task<bool> RemoveProductFromCartAsync(int cartId, int productId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartId, productId);
            if (cartItem == null)
                return false;

            // Get the product from the database
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return false;

            // Add the quantity back to the product stock
            product.Quantity += cartItem.Quantity;

            // Remove the cart item from the cart
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
