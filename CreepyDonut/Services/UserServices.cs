using Microsoft.EntityFrameworkCore;
using CreepyDonut.Data;
using CreepyDonut.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreepyDonut.DTO;

namespace CreepyDonut.Services
{
    public class UserService
    {
        private readonly ApiContext _context;

        public UserService(ApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Users>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<Users?> GetAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<Users> CreateAsync(UsersDTO userDto)
        {
            var user = new Users
            {
                UserId = userDto.UserId,
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = userDto.Password,
                PhoneNumber = userDto.PhoneNumber,
                CreatedAt = userDto.CreatedAt
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateAsync(int id, Users user)
        {
            if (id != user.UserId)
                return false;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExists(id))
                    return false;
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(e => e.UserId == id);
        }
    }
}

