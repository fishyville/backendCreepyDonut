namespace CreepyDonut.Models
{
    public class ProductShop
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int ShopId { get; set; }
        public Shop Shop { get; set; } = null!;
    }
}
