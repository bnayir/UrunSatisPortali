using System.ComponentModel.DataAnnotations;

namespace UrunSatisPortali.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Marka adı zorunludur.")]
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}