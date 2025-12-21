using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

namespace UrunSatisPortali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
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
            var products = _productRepo.GetAll("Category");
            return View(products);
        }

        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_categoryRepo.GetAll(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedDate = DateTime.Now;
                _productRepo.Add(product);
                // SADECE YÖNLENDİRME (Mesaj Yok)
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryId = new SelectList(_categoryRepo.GetAll(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        public IActionResult Edit(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null) return NotFound();

            ViewBag.CategoryId = new SelectList(_categoryRepo.GetAll(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            var dbProduct = _productRepo.GetById(product.Id);
            if (dbProduct == null) return NotFound();

            dbProduct.Name = product.Name;
            dbProduct.Description = product.Description;
            dbProduct.Price = product.Price;
            dbProduct.Stock = product.Stock;
            dbProduct.IsActive = product.IsActive;
            dbProduct.CategoryId = product.CategoryId;
            dbProduct.Image = product.Image;
            dbProduct.UpdatedDate = DateTime.Now;

            _productRepo.Update(dbProduct);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null) return Json(new { success = false });

            _productRepo.Delete(product);
            return Json(new { success = true });
        }
    }
}