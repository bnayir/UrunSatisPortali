using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

namespace UrunSatisPortali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] // Sadece giriş yapanlar görebilsin
    public class DashboardController : Controller
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Category> _categoryRepo;

        public DashboardController(IRepository<Product> productRepo, IRepository<Category> categoryRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
        }

        public IActionResult Index()
        {
            // Veritabanındaki toplam sayıları alıp ViewBag ile sayfaya taşıyoruz
            ViewBag.ProductCount = _productRepo.GetAll().Count();
            ViewBag.CategoryCount = _categoryRepo.GetAll().Count();

            return View();
        }
    }
}