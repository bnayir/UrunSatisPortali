namespace UrunSatisPortali.Models
{
    public class Newsletter
    {
        public int Id { get; set; } // Birincil anahtar
        public string Email { get; set; } // Abone e-postası
        public DateTime CreatedDate { get; set; } = DateTime.Now; // Kayıt tarihi
    }
}