namespace CreepyDonut.DTO
{
    public class AddProductRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1; // Optional: default to 1
    }

    public class updateQuantityRequest
    {
        public int Quantity { get; set; }
    }

    public class CartDto
    {
        public int Id { get; set; }
        public required List<CartItemDto> Items { get; set; }
    }

    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
        public string ImageUrl { get; set; }      // New
        public int? CategoryId { get; set; }
    }
}
