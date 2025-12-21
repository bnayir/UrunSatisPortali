using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

namespace UrunSatisPortali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CommentController : Controller
    {
        private readonly IRepository<Comment> _commentRepo;

        public CommentController(IRepository<Comment> commentRepo)
        {
            _commentRepo = commentRepo;
        }

        public IActionResult Index()
        {
            // Yorumları, hangi ürüne ve hangi kullanıcıya ait olduğu bilgisiyle çekiyoruz
            var comments = _commentRepo.GetAll("Product,User").OrderByDescending(x => x.CreatedDate).ToList();
            return View(comments);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var comment = _commentRepo.GetById(id);
            if (comment == null) return Json(new { success = false });

            _commentRepo.Delete(comment);
            return Json(new { success = true });
        }
    }
}