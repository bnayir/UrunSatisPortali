using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrunSatisPortali.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? Image { get; set; }

        [Display(Name = "Aktif mi?")]
        public bool IsActive { get; set; }
        [Display(Name = "Kayıt Tarihi")]
        public DateTime CreatedDate { get; set; } = DateTime.Now; // Varsayılan olarak şu an

        [Display(Name = "Güncelleme Tarihi")]
        public DateTime? UpdatedDate { get; set; }
        // Foreign Key ve Navigation Property
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }
        // Product.cs içine eklenecekler:
        public int? BrandId { get; set; } // Foreign Key
        [ForeignKey("BrandId")]
        [ValidateNever]
        public Brand Brand { get; set; } // Navigation Property
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
