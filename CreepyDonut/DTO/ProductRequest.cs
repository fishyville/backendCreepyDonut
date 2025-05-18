namespace CreepyDonut.DTO

{
    public class NewProductDTO
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
        public required string ImageUrl { get; set; }
        public required int Quantity { get; set; }
        public required int CategoryId { get; set; }

    }

    public class ProductResponseDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }

}
