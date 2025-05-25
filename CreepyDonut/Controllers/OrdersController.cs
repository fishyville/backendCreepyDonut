using CreepyDonut.DTO;
using CreepyDonut.Services;
using Microsoft.AspNetCore.Mvc;

namespace CreepyDonut.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderServices _service;

        public OrdersController(OrderServices service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }
        // CREATE ORDER
        [HttpPost]
        public async Task<IActionResult> Create(OrderRequest req)
        {
            var created = await _service.CreateAsync(req);
            return CreatedAtAction(nameof(GetById), new { id = created.OrderId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderRequest req)
        {
            var updated = await _service.UpdateAsync(id, req);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpPatch("{orderId}/status")]
        public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] UpdateOrderStatusRequest req)
        {
            var updated = await _service.UpdateStatusAsync(orderId, req.Status);
            if (!updated) return NotFound();

            return Ok(new { message = "Status updated successfully" });
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<OrderResponse>>> GetOrdersByUserId(int userId)
        {
            var orders = await _service.GetOrderResponsesByUserIdAsync(userId);
            if (orders == null || !orders.Any())
                return NotFound();

            return Ok(orders);
        }
    }
}
