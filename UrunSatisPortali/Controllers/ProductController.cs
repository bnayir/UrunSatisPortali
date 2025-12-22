using Microsoft.AspNetCore.Identity; // Bunu eklediğinden emin ol
using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

namespace UrunSatisPortali.Controllers
{
    public class ProductController : Controller
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Comment> _commentRepo;
        private readonly UserManager<IdentityUser> _userManager; // <IdentityUser> eklendi

        public ProductController(
            IRepository<Product> productRepo,
            IRepository<Comment> commentRepo,
            UserManager<IdentityUser> userManager) // Buraya da eklendi
        {
            _productRepo = productRepo;
            _commentRepo = commentRepo;
            _userManager = userManager;
        }

        public IActionResult Details(int id)
        {
            // "Comments" tablosunu Include (dahil) ediyoruz
            var product = _productRepo.GetAll("Category,Brand,Comments").FirstOrDefault(x => x.Id == id);

            if (product == null) return NotFound();

            ViewBag.RelatedProducts = _productRepo.GetAll("Category,Brand")
                .Where(x => x.CategoryId == product.CategoryId && x.Id != id)
                .Take(4)
                .ToList();

            return View(product);
        }

        [HttpPost]
        public IActionResult AddComment(int ProductId, string Content)
        {
            // Kullanıcı ID'sini almak için doğru kullanım
            var userId = _userManager.GetUserId(User);

            if (!string.IsNullOrEmpty(Content) && userId != null)
            {
                var comment = new Comment
                {
                    ProductId = ProductId,
                    Content = Content,
                    UserId = userId,
                    CreatedDate = DateTime.Now
                };
                _commentRepo.Add(comment);
            }
            return RedirectToAction("Details", new { id = ProductId });
        }
    }
}