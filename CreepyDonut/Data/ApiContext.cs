using CreepyDonut.Models;
using Microsoft.EntityFrameworkCore;


namespace CreepyDonut.Data
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options) {}

        public DbSet<Users> Users { get; set; }
    }
}
