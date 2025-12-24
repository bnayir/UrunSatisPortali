namespace UrunSatisPortali.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // Toplam fiyatı otomatik hesaplayan özellik
        public decimal TotalPrice => Price * Quantity;
        public string ProductName => Product?.Name;
    }
}