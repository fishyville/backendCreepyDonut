using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CreepyDonut.Models
{
    public class Cart
    {

        [Key]
        public int CartId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public required Users User { get; set; }  // Navigation to User

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();  // Navigation to CartItems

    }
}
