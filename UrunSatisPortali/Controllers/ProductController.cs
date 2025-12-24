using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;
using System.Linq;
using System;

namespace UrunSatisPortali.Controllers
{
    public class ProductController : Controller
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Comment> _commentRepo;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductController(
            IRepository<Product> productRepo,
            IRepository<Comment> commentRepo,
            UserManager<IdentityUser> userManager)
        {
            _productRepo = productRepo;
            _commentRepo = commentRepo;
            _userManager = userManager;
        }

        public IActionResult Details(int id)
        {
            // Mevcut ürünü tüm ilişkili tablolarıyla birlikte çekiyoruz
            var product = _productRepo.GetAll("Category,Brand,Comments.User").FirstOrDefault(x => x.Id == id);

            if (product == null) return NotFound();

            // ÖNEMLİ: Details sayfasında görünecek öneri ürünlerini hazırlıyoruz
            ViewBag.SuggestedProducts = _productRepo.GetAll("Category,Brand")
                                                    .Where(x => x.Id != id) // Bakılan ürünü listeden çıkar
                                                    .OrderBy(x => Guid.NewGuid())
                                                    .Take(4)
                                                    .ToList();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Rating parametresi View'dan gelen veriyi yakalamak için eklendi
        public IActionResult AddComment(int ProductId, string Content, int Rating)
        {
            // Kullanıcı girişi kontrolü
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            var userId = _userManager.GetUserId(User);

            // Gelen verilerin geçerlilik kontrolü
            if (!string.IsNullOrEmpty(Content) && userId != null)
            {
                var comment = new Comment
                {
                    ProductId = ProductId,
                    Content = Content,
                    Rating = Rating > 0 ? Rating : 5, 
                    UserId = userId,
                    CreatedDate = DateTime.Now
                };

                _commentRepo.Add(comment); // Veritabanına kayıt işlemi
            }

            // Sayfayı yenileyerek yorumu göster
            return RedirectToAction("Details", new { id = ProductId });
        }
    }
}