using System;
using System.Collections.Generic;

namespace UrunSatisPortali.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Siparişi veren kullanıcı ID'si
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        // Teslimat Bilgileri
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }

        // Eğer sipariş kalemlerini (ürünleri) tutmak isterseniz:
        // public List<OrderItem> OrderItems { get; set; }
    }
}