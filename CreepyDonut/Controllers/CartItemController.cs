using Microsoft.AspNetCore.Mvc;
using CreepyDonut.Models;
using CreepyDonut.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CreepyDonut.Controllers
{
    [Route("api/cart-items")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly CartItemService _cartItemService;

        public CartItemController(CartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        // GET ALL ITEMS IN A CART
        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetCartItems(int cartId)
        {
            var cartItems = await _cartItemService.GetCartItemsAsync(cartId);
            if (cartItems == null || cartItems.Count == 0)
                return NotFound(new { message = "No items found in the cart" });

            return Ok(cartItems);
        }

        // ADD ITEM TO CART
        [HttpPost("add")]
        public async Task<IActionResult> AddCartItem([FromBody] CartItem cartItem)
        {
            var success = await _cartItemService.AddCartItemAsync(cartItem.CartId, cartItem.ProductId, cartItem.Quantity);
            if (!success)
                return BadRequest(new { message = "Failed to add item to cart" });

            return Ok(new { message = "Item added to cart" });
        }

        // UPDATE CART ITEM QUANTITY
        [HttpPut("update/{cartItemId}")]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, [FromBody] int quantity)
        {
            var success = await _cartItemService.UpdateCartItemAsync(cartItemId, quantity);
            if (!success)
                return NotFound(new { message = "Cart item not found" });

            return Ok(new { message = "Cart item updated successfully" });
        }

        // REMOVE ITEM FROM CART
        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            var success = await _cartItemService.RemoveCartItemAsync(cartItemId);
            if (!success)
                return NotFound(new { message = "Cart item not found" });

            return Ok(new { message = "Cart item removed successfully" });
        }
    }
}
