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

        // GET ALL USERS
        public async Task<IEnumerable<Users>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }


        // GET USER BY ID
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


        // PASSWORD HASH FUNCTION
        private string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32
            ));

            return $"{Convert.ToBase64String(salt)}.{hashed}"; // Store salt + hash
        }

        // AUTHENTICATION WHEN LOGIN
        public async Task<Users?> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return null;

            if (!VerifyPassword(password, user.PasswordHash))
                return null;

            return user; // Authentication successful
        }

        // BUAT NGOCOKIN PASSWORD
        private bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split('.');
            if (parts.Length != 2)
                return false; // Invalid stored hash format

            byte[] salt = Convert.FromBase64String(parts[0]);
            string storedPasswordHash = parts[1];

            string inputHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32
            ));

            return storedPasswordHash == inputHash; // Compare hashes
        }

        // UPDATE USER 
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


        // DELETE USER
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // CHECK USER IF EXIST
        private async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(e => e.UserId == id);
        }

        // CHECK EMAIL KALO ADA YANG SAMA ATAU GAK
        public async Task<Users?> GetByUsernameOrEmailAsync(string username, string email)
        {
            return await _context.Users
                .SingleOrDefaultAsync(u => u.Username == username || u.Email == email);
        }

    }
}

