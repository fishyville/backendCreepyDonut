using Microsoft.AspNetCore.Mvc;
using CreepyDonut.Models;
using CreepyDonut.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CreepyDonut.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetAllAsync()
        {
            return Ok(await _userService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetAsync(int id)
        {
            var user = await _userService.GetAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<Users>> PostAsync(Users user)
        {
            var createdUser = await _userService.CreateAsync(user);
            return CreatedAtAction(nameof(GetAsync), new { id = createdUser.UserId }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, Users user)
        {
            if (!await _userService.UpdateAsync(id, user))
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (!await _userService.DeleteAsync(id))
                return NotFound();
            return NoContent();
        }
    }
}
