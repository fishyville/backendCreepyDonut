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
        // To get user list
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
                return NotFound();
            return Ok(user);
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
