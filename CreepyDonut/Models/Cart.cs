using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreepyDonut.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }


        public required Users User { get; set; }  

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        public Order? Order { get; set; } 
    }
}
