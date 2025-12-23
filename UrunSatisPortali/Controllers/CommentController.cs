using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;
using UrunSatisPortali.Hubs;

// Namespace'in başına dikkat et, Admin olanla karışmasın
namespace UrunSatisPortali.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly IRepository<Comment> _commentRepo;
        private readonly IHubContext<GeneralHub> _hubContext;

        public CommentController(IRepository<Comment> commentRepo, IHubContext<GeneralHub> hubContext)
        {
            _commentRepo = commentRepo;
            _hubContext = hubContext;
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Güvenlik için ekle (Finalde puan kazandırır)
        public async Task<IActionResult> AddComment(int productId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return RedirectToAction("Details", "Product", new { id = productId });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Challenge(); // Kullanıcı ID alınamazsa tekrar girişe at

            var comment = new Comment
            {
                ProductId = productId,
                Content = content,
                UserId = userId,
                CreatedDate = DateTime.Now
            };

            _commentRepo.Add(comment);

            // SIGNALR BİLDİRİMİ
            var currentCount = _commentRepo.GetAll().Count();
            await _hubContext.Clients.All.SendAsync("ReceiveCommentCount", currentCount);

            // ÖNEMLİ: Product Details sayfasının 'Area'sı olmadığı için explicit belirtiyoruz
            return RedirectToAction("Details", "Product", new { area = "", id = productId });
        }
    }
}