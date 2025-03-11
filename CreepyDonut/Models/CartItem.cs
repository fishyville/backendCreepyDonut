using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CreepyDonut.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [ForeignKey("Cart")]
        public int CartId { get; set; }
        public Cart? Cart { get; set; }  // Navigation to Cart

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }  // Navigation to Product

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
