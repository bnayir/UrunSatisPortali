using System.ComponentModel.DataAnnotations;

namespace UrunSatisPortali.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Kayıt Tarihi")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Güncelleme Tarihi")]
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
