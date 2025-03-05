using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreepyDonut.Models
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(255)]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public required string Email { get; set; }

        [Required]
        [StringLength(255)]
        public required string PasswordHash { get; set; }

        [StringLength(20)]
        public required string PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

