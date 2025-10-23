using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; 
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

namespace UrunSatisPortali.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Category> _categoryRepo;

        public ProductController(IRepository<Product> productRepo, IRepository<Category> categoryRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
        }

        public IActionResult Index()
        {
            var products = _productRepo.GetAll();
            return View(products);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_categoryRepo.GetAll(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            ModelState.Remove("Category");
            if (ModelState.IsValid)
            {
                _productRepo.Add(product);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_categoryRepo.GetAll(), "Id", "Name");
            return View(product);
        }

        public IActionResult Edit(int id)
        {
            var product = _productRepo.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_categoryRepo.GetAll(), "Id", "Name", product.CategoryId);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                _productRepo.Update(product);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_categoryRepo.GetAll(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Hata: Ürün bulunamadı." });
            }

            _productRepo.Delete(product);

            return Json(new { success = true, message = "Ürün başarıyla silindi." });
        }


    }
}