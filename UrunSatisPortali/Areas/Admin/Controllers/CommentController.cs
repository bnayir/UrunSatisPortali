using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;
using UrunSatisPortali.Hubs;
using System.Security.Claims; // User ID'yi çekmek için gerekli

namespace UrunSatisPortali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] // Genel olarak giriş şartı
    public class CommentController : Controller
    {
        private readonly IRepository<Comment> _commentRepo;
        private readonly IHubContext<DashboardHub> _hubContext;

        public CommentController(IRepository<Comment> commentRepo, IHubContext<DashboardHub> hubContext)
        {
            _commentRepo = commentRepo;
            _hubContext = hubContext;
        }

        // --- ADMİN LİSTELEME ---
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var comments = _commentRepo.GetAll("Product,User").OrderByDescending(x => x.CreatedDate).ToList();
            return View(comments);
        }

        // --- YENİ YORUM EKLEME (BURAYI EKLE) ---
        [HttpPost]
        public async Task<IActionResult> AddComment(int productId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return RedirectToAction("Details", "Product", new { id = productId });

            var comment = new Comment
            {
                ProductId = productId,
                Content = content, // Senin View tarafındaki ismin: content
                CreatedDate = DateTime.Now,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) // Giriş yapan kullanıcının ID'si
            };

            _commentRepo.Add(comment);

            // SİNYAL GÖNDERME: Dashboard'daki sayıyı anlık artır
            var currentCommentCount = _commentRepo.GetAll().Count();
            await _hubContext.Clients.All.SendAsync("ReceiveCommentCount", currentCommentCount);

            // Ürün detay sayfasına geri dön
            return RedirectToAction("Details", "Product", new { area = "", id = productId });
        }

        // --- YORUM SİLME ---
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = _commentRepo.GetById(id);
            if (comment == null) return Json(new { success = false });

            _commentRepo.Delete(comment);

            var currentCommentCount = _commentRepo.GetAll().Count();
            await _hubContext.Clients.All.SendAsync("ReceiveCommentCount", currentCommentCount);

            return Json(new { success = true });
        }
    }
}