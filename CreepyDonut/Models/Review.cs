using CreepyDonut.Models;

public class Review
{
    public int ReviewId { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int UserId { get; set; }
    public Users User { get; set; } = null!;

    public double Rating { get; set; }
    public string? ReviewText { get; set; }
}
