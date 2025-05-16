namespace CreepyDonut.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public required string ImageUrl { get; set; }
        public int Quantity { get; set; }

        // Many-to-One: Product -> Category
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // Many-to-Many: Product <-> Shop (via ProductShop join table)
        public List<ProductShop> ProductShops { get; set; } = new();

        // One-to-Many: Product -> Reviews
        public List<Review> Reviews { get; set; } = new();

        // One-to-Many: Product -> CartItems
        public List<CartItem> CartItems { get; set; } = new();
    }
}

