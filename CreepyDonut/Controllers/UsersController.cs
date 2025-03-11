using Microsoft.AspNetCore.Mvc;
using CreepyDonut.Models;
using CreepyDonut.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreepyDonut.DTO;


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
        // AMBIL SEMUA USER INFO
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetAllAsync()
        {
            var users = await _userService.GetAllAsync();
            return Ok(new
            {
                success = true,
                message = "Users retrieved successfully.",
                data = users
            });
        }

        // AMBIL USER INFO DARI ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetAsync(int id)
        {
            var user = await _userService.GetAsync(id);
            if (user == null)
            {
                return NotFound(new { success = false, message = "User not found." });
            }
            return Ok(new
            {
                success = true,
                message = "User retrieved successfully.",
                data = user
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] Users user)
        {
            // Check if username or email already exists
            var existingUser = await _userService.GetByUsernameOrEmailAsync(user.Username, user.Email);
            if (existingUser != null)
                return BadRequest(new { message = "Username or email already in use" });

            // Create the user with hashed password
            var createdUser = await _userService.CreateAsync(user);
            return Ok(new { message = "Registration successful", userId = createdUser.UserId });
        }


        // TAMBAH USER (ADMIN) (JANGAN UBAH ID KALO TAMBAH USER NANTI FAILED)
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Users user)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data." });

            var createdUser = await _userService.CreateAsync(user);

            return CreatedAtAction(nameof(GetAsync), new { id = createdUser.UserId },
                new { success = true, message = "User created successfully." });
        }


        // LOGIN (CUSTOMER)
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            var user = await _userService.AuthenticateAsync(request.Username, request.Password);
            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(new { message = "Login successful", userId = user.UserId });
        }


        // UPDATE USER SESUAI ID 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Users user)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data." });

            var updated = await _userService.UpdateAsync(id, user);
            if (!updated)
                return NotFound(new { success = false, message = "User not found." });

            return Ok(new { success = true, message = "User updated successfully." });
        }

        // DELETE USER SESUAI ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { success = false, message = "User not found." });

            return Ok(new { success = true, message = "User deleted successfully." });
        }
    }
}
