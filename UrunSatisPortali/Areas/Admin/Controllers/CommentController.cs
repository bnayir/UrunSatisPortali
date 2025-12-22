using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;
using UrunSatisPortali.Hubs;
using System.Security.Claims;

namespace UrunSatisPortali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CommentController : Controller
    {
        private readonly IRepository<Comment> _commentRepo;
        private readonly IHubContext<DashboardHub> _hubContext;

        public CommentController(IRepository<Comment> commentRepo, IHubContext<DashboardHub> hubContext)
        {
            _commentRepo = commentRepo;
            _hubContext = hubContext;
        }

        // --- ADMİN LİSTELEME (DEĞERLENDİRME PUANI DAHİL) ---
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            // Product ve User verilerini dahil ederek listeliyoruz
            var comments = _commentRepo.GetAll("Product,User")
                                       .OrderByDescending(x => x.CreatedDate)
                                       .ToList();
            return View(comments);
        }

        // --- YENİ YORUM VE DEĞERLENDİRME EKLEME ---
        [HttpPost]
        public async Task<IActionResult> AddComment(int productId, string content, int rating)
        {
            if (string.IsNullOrWhiteSpace(content))
                return RedirectToAction("Details", "Product", new { area = "", id = productId });

            var comment = new Comment
            {
                ProductId = productId,
                Content = content,
                Rating = rating > 0 ? rating : 5, // Gelen puanı kaydediyoruz
                CreatedDate = DateTime.Now,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            _commentRepo.Add(comment);

            // SİNYAL GÖNDERME: Dashboard'daki sayıyı anlık artır
            var currentCommentCount = _commentRepo.GetAll().Count();
            await _hubContext.Clients.All.SendAsync("ReceiveCommentCount", currentCommentCount);

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

            // SİNYAL GÖNDERME: Dashboard'daki sayıyı güncelle
            var currentCommentCount = _commentRepo.GetAll().Count();
            await _hubContext.Clients.All.SendAsync("ReceiveCommentCount", currentCommentCount);

            return Json(new { success = true });
        }
    }
}