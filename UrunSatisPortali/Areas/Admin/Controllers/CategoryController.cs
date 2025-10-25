using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

namespace UrunSatisPortali.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Product> _productRepo;
        public CategoryController(IRepository<Category> categoryRepo, IRepository<Product> productRepo)
        {
            _categoryRepo = categoryRepo;
            _productRepo = productRepo;
        }
        public ActionResult Index()
        {
            var categories = _categoryRepo.GetAll();
            return View(categories);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepo.Add(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public IActionResult Edit(int id)
        {
            var category = _categoryRepo.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepo.Update(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            bool hasProducts = _productRepo.GetAll().Any(p => p.CategoryId == id);

            if (hasProducts)
            {
                return Json(new { success = false, message = "Bu kategoriye bağlı ürünler varken silemezsiniz!" });
            }

            var category = _categoryRepo.GetById(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Hata: Kategori bulunamadı." });
            }

            _categoryRepo.Delete(category);

            return Json(new { success = true, message = "Kategori başarıyla silindi." });
        }

    }
}
