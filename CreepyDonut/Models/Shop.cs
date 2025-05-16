using CreepyDonut.Models;

public class Shop
{
    public int ShopId { get; set; }
    public string Location { get; set; } = null!;

    // Many-to-Many via join table
    public List<ProductShop> ProductShops { get; set; } = new();
}
