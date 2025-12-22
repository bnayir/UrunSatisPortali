using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

namespace UrunSatisPortali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MessageController : Controller
    {
        private readonly IRepository<Message> _messageRepo;

        public MessageController(IRepository<Message> messageRepo)
        {
            _messageRepo = messageRepo;
        }

        // Mesajları Listeleme
        public IActionResult Index()
        {
            var messages = _messageRepo.GetAll().OrderByDescending(x => x.CreatedDate).ToList();
            return View(messages);
        }

        // Mesaj Silme
        public IActionResult Delete(int id)
        {
            var msg = _messageRepo.GetById(id);
            if (msg != null)
            {
                _messageRepo.Delete(msg);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}