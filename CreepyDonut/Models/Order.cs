using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CreepyDonut.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

  
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual required Users User { get; set; }

        [Required]
        public int CartId { get; set; }

        [ForeignKey("CartId")]
        public virtual required Cart Cart { get; set; }

        public decimal TotalPrice { get; set; }

        public required string Status { get; set; } = "Unpaid";

        public  string? PaymentMethod { get; set; } = null;

        public required string ShippingAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
