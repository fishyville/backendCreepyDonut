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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsersDTO>>> GetAllAsync()
        {
            var users = await _userService.GetAllAsync();
            var userDtos = users.Select(static user => new UsersDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Password = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt
            });
            Console.WriteLine("✅ Success: Fetched all users at " + DateTime.Now);
            return Ok(userDtos);
        }



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



        // for register
        [HttpPost]
        public async Task<ActionResult<UsersDTO>> PostAsync(RegisterDTO registerDto)
        {
            var newUser = new UsersDTO
            {
                UserId = 0,
                Username = registerDto.Username,
                Email = registerDto.Email,
                Password = registerDto.Password,   
                PhoneNumber = registerDto.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userService.CreateAsync(newUser);

            var userDto = new UsersDTO
            {
                UserId = createdUser.UserId,
                Username = createdUser.Username,
                Email = createdUser.Email,
                Password = createdUser.PasswordHash,  
                PhoneNumber = createdUser.PhoneNumber,
                CreatedAt = createdUser.CreatedAt
            };

            return CreatedAtAction(nameof(GetAsync), new { id = userDto.UserId }, userDto);
        }

        [HttpPost("login-username")]
        public async Task<ActionResult<UsersDTO>> LoginWithUsername([FromBody] LoginRequestUsername loginDto)
        {
            var users = await _userService.GetAllAsync();
            var user = users.FirstOrDefault(u =>
                u.Username == loginDto.Username && u.PasswordHash == loginDto.Password);

            if (user == null)
            {
                return Unauthorized("❌ Invalid username or password.");
            }

            Console.WriteLine($"✅ User '{user.Username}' logged in successfully with username.");

            var userDto = new UsersDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Password = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt
            };

            return Ok(userDto);
        }

        [HttpPost("login-email")]
        public async Task<ActionResult<UsersDTO>> LoginWithEmail([FromBody] LoginRequestEmail loginDto)
        {
            var users = await _userService.GetAllAsync();
            var user = users.FirstOrDefault(u =>
                u.Email == loginDto.Email && u.PasswordHash == loginDto.Password);

            if (user == null)
            {
                return Unauthorized("❌ Invalid email or password.");
            }

            Console.WriteLine($"✅ User '{user.Email}' logged in successfully with email.");

            var userDto = new UsersDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Password = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt
            };

            return Ok(userDto);

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
