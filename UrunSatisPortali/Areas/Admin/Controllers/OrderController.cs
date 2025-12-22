using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;
using System.Linq;
using System.Threading.Tasks;

namespace UrunSatisPortali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IRepository<Order> _orderRepo;

        public OrderController(IRepository<Order> orderRepo)
        {
            _orderRepo = orderRepo;
        }

        // Siparişleri Listeleme
        public IActionResult Index()
        {
            var allOrders = _orderRepo.GetAll("User")
                                      .OrderByDescending(x => x.OrderDate)
                                      .ToList();
            return View(allOrders);
        }

        // SİPARİŞ DURUMUNU GÜNCELLEME
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int id, string status)
        {
            if (string.IsNullOrEmpty(status)) return BadRequest();

            // Veritabanından mevcut kaydı getir
            var order = _orderRepo.GetById(id);
            if (order == null) return NotFound();

            // Sadece Status alanını güncelle
            order.Status = status;

            // Repository üzerinden SaveChanges içeren Update'i çağır
            _orderRepo.Update(order);

            // Başarılı mesajı ile geri dön (Opsiyonel)
            TempData["Success"] = "Sipariş durumu güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // SİPARİŞ DETAYI (Kullanıcı Tarafı ve Admin İçin)
        public IActionResult Details(int id)
        {
            // Kullanıcı verisiyle birlikte getir
            var order = _orderRepo.GetAll("User").FirstOrDefault(x => x.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }
    }
}