using Microsoft.AspNetCore.Mvc;
using CreepyDonut.Models;
using CreepyDonut.Services;
using System.Threading.Tasks;

namespace CreepyDonut.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        // GET CART BY USER ID
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
                return NotFound(new { message = "Cart not found for user" });

            return Ok(cart);
        }

        // CREATE CART FOR USER
        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateCart(int userId)
        {
            var cart = await _cartService.CreateCartAsync(userId);
            return CreatedAtAction(nameof(GetCart), new { userId = cart.UserId }, cart);
        }

        // ADD PRODUCT TO CART (Checks if cart exists, otherwise creates one)
        [HttpPost("{userId}/add-product")]
        public async Task<IActionResult> AddProductToCart(int userId, [FromBody] CartItem cartItem)
        {
            // Check if user already has a cart
            var cart = await _cartService.GetCartByUserIdAsync(userId);

            // If no cart exists, create one
            if (cart == null)
            {
                cart = await _cartService.CreateCartAsync(userId);
            }

            // Add product to the cart
            var success = await _cartService.AddProductToCartAsync(cart.CartId, cartItem.ProductId, cartItem.Quantity);
            if (!success)
                return BadRequest(new { message = "Failed to add product to cart" });

            return Ok(new { message = "Product added to cart", cartId = cart.CartId });
        }

        // UPDATE CART ITEM QUANTITY
        [HttpPut("update-quantity/{cartItemId}")]
        public async Task<IActionResult> UpdateCartItemQuantity(int cartItemId, [FromBody] int quantity)
        {
            var success = await _cartService.UpdateCartItemQuantityAsync(cartItemId, quantity);
            if (!success)
                return NotFound(new { message = "Cart item not found" });

            return Ok(new { message = "Cart item quantity updated" });
        }

        // REMOVE PRODUCT FROM CART
        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveProductFromCart(int cartItemId)
        {
            var success = await _cartService.RemoveProductFromCartAsync(cartItemId);
            if (!success)
                return NotFound(new { message = "Cart item not found" });

            return Ok(new { message = "Product removed from cart" });
        }

        // CLEAR CART
        [HttpDelete("clear/{cartId}")]
        public async Task<IActionResult> ClearCart(int cartId)
        {
            var success = await _cartService.ClearCartAsync(cartId);
            if (!success)
                return NotFound(new { message = "Cart not found" });

            return Ok(new { message = "Cart cleared successfully" });
        }
    }
}
