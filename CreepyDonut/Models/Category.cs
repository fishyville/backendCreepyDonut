namespace CreepyDonut.Models
{
    public class Category
    {
        public int CategoryId { get; set; }  // Primary key
        public string Name { get; set; } = null!;

        // One category has many products
        public List<Product> Products { get; set; } = new();
    }
}

