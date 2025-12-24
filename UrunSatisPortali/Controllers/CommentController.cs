using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;
using UrunSatisPortali.Hubs;

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
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> AddComment(int productId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return RedirectToAction("Details", "Product", new { id = productId });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Challenge(); 

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

            return RedirectToAction("Details", "Product", new { area = "", id = productId });
        }
    }
}