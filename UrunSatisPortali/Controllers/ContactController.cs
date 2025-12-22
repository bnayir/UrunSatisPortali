using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

namespace UrunSatisPortali.Controllers
{
    public class ContactController : Controller
    {
        private readonly IRepository<Message> _messageRepo;

        public ContactController(IRepository<Message> messageRepo)
        {
            _messageRepo = messageRepo;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult SendMessage(Message model)
        {
            if (ModelState.IsValid)
            {
                // 1. Gerekli alanları doldur
                model.CreatedDate = DateTime.Now;
                model.IsRead = false;

                // 2. Mesajı ekle
                _messageRepo.Add(model);

                TempData["SuccessMessage"] = "Mesajınız başarıyla iletildi.";
                return RedirectToAction("Index");
            }
            // Hata varsa sayfayı formuyla birlikte geri döndür
            return View("Index", model);
        }
    }
}