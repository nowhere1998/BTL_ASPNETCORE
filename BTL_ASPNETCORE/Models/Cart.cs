namespace BTL_ASPNETCORE.Models
{
    public class Cart
    { 
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public int Quantity { get; set; } = 1;
        public float Price { get; set; } = 0;
    }
}
