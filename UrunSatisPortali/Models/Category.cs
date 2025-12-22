using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrunSatisPortali.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; }

        [Display(Name = "Kayıt Tarihi")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Güncelleme Tarihi")]
        public DateTime? UpdatedDate { get; set; }

        public bool IsActive { get; set; } = true;

        // --- ALT KATEGORİ DESTEĞİ İÇİN EKLENENLER ---

        [Display(Name = "Üst Kategori")]
        public int? ParentId { get; set; } // Eğer null ise bu bir ana kategoridir.

        [ForeignKey("ParentId")]
        public virtual Category? Parent { get; set; } // Üst kategorinin kendisi

        public virtual ICollection<Category>? SubCategories { get; set; } // Bu kategorinin altındakiler

        // Bu kategoriye ait ürünler
        public virtual ICollection<Product>? Products { get; set; }
    }
}