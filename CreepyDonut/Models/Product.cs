namespace CreepyDonut.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public required string ImageUrl { get; set; }
        public int QuantityAvailable { get; set; }  // Adjusted field name

        // One Product -> Many CartItems (Many-to-Many via CartItem)
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
