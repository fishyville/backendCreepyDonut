using Microsoft.AspNetCore.Mvc;
using CreepyDonut.Models;
using CreepyDonut.Services;
using System.Threading.Tasks;
using CreepyDonut.DTO;

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
        public async Task<IActionResult> GetCartByUserId(int userId)
        {
            var cartDto = await _cartService.GetCartDtoByUserIdAsync(userId);
            if (cartDto == null)
                return NotFound("Cart not found");

            return Ok(cartDto);
        }

        // CREATE CART FOR USER
        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateCart(int userId)
        {
            var cart = await _cartService.CreateCartAsync(userId);
            return CreatedAtAction(nameof(GetCartByUserId), new { userId = cart.UserId }, cart);
        }

        // ADD PRODUCT TO CART (Checks if cart exists, otherwise creates one)
        [HttpPost("{userId}/add-product")]
        public async Task<IActionResult> AddProductToCart(int userId, [FromBody] AddProductRequest request)
        {
            // Check if user already has a cart
            var cart = await _cartService.GetCartByUserIdAsync(userId);

            // If no cart exists, create one
            if (cart == null)
            {
                cart = await _cartService.CreateCartAsync(userId);
            }

            // Add product to the cart
            var success = await _cartService.AddProductToCartAsync(cart.CartId, request.ProductId, request.Quantity);
            if (!success)
                return BadRequest(new { message = "Failed to add product to cart" });

            return Ok(new { message = "Product added to cart", cartId = cart.CartId });
        }

        // UPDATE CART ITEM QUANTITY
        [HttpPut("update-quantity/{cartId}/{productId}")]
        public async Task<IActionResult> UpdateCartItemQuantity(int cartId, int productId, [FromBody] updateQuantityRequest request)
        {
            if (request.Quantity == 0)
            {
                var removed = await _cartService.RemoveProductFromCartAsync(cartId, productId);
                if (!removed)
                    return NotFound(new { message = "Cart item not found to remove" });

                return Ok(new { message = "Cart item removed because quantity was set to 0" });
            }

            var success = await _cartService.UpdateCartItemQuantityAsync(cartId, productId, request.Quantity);
            if (!success)
                return NotFound(new { message = "Cart item not found" });

            return Ok(new { message = "Cart item quantity updated" });
        }


        // REMOVE PRODUCT FROM CART
        [HttpDelete("{cartId}/product/{productId}")]
        public async Task<IActionResult> RemoveProductFromCart(int cartId, int productId)
        {
            var result = await _cartService.RemoveProductFromCartAsync(cartId, productId);
            if (!result)
                return NotFound();

            return Ok("Product removed from cart.");
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
