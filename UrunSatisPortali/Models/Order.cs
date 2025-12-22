using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity; // IdentityUser için gerekli

namespace UrunSatisPortali.Models
{
    public class Order
    {
        public int Id { get; set; }

        // --- KULLANICI İLİŞKİSİ ---
        public string UserId { get; set; } // Siparişi veren kullanıcı ID'si

        // Navigation Property: View tarafında @item.User.UserName diyebilmek için bu şarttır.
        public virtual IdentityUser User { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        // Teslimat Bilgileri
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }

        // Dinamik Durum Alanı
        public string Status { get; set; } = "Sipariş Alındı"; // Varsayılan değer
    }
}