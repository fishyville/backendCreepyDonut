namespace CreepyDonut.DTO
{
    public class OrderRequest
    {
        public int UserId { get; set; }
        public int CartId { get; set; }
        public decimal TotalPrice { get; set; }
        public required string Status { get; set; }
        public required string PaymentMethod { get; set; }
        public required string ShippingAddress { get; set; }
    }

    public class OrderResponse
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int CartId { get; set; }
        public decimal TotalPrice { get; set; }
        public required string Status { get; set; }
        public required string PaymentMethod { get; set; }
        public required string ShippingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class UpdateOrderStatusRequest
    {
        public required string Status { get; set; }
    }
}
