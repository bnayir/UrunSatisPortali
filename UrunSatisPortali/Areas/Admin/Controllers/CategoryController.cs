using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR; 
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;
using UrunSatisPortali.Hubs; 

namespace UrunSatisPortali.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly IHubContext<GeneralHub> _hubContext; // SignalR Bağlantısı

        public CategoryController(IRepository<Category> categoryRepo, IRepository<Product> productRepo, IHubContext<GeneralHub> hubContext)
        {
            _categoryRepo = categoryRepo;
            _productRepo = productRepo;
            _hubContext = hubContext;
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
        public async Task<IActionResult> Create(Category category) 
        {
            if (ModelState.IsValid)
            {
                _categoryRepo.Add(category);

                // --- SIGNALR TETİKLEME ---
                await _hubContext.Clients.All.SendAsync("onCategoryAdd", "Yeni bir kategori eklendi: " + category.Name);

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
        public async Task<IActionResult> Edit(Category category) 
        {
            if (ModelState.IsValid)
            {
                category.UpdatedDate = DateTime.Now;
                _categoryRepo.Update(category);

                // --- SIGNALR TETİKLEME ---
                await _hubContext.Clients.All.SendAsync("onCategoryUpdate", "Kategori güncellendi: " + category.Name);

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id) 
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

            // --- SIGNALR TETİKLEME ---
            await _hubContext.Clients.All.SendAsync("onCategoryDelete", "Kategori başarıyla silindi.");

            return Json(new { success = true, message = "Kategori başarıyla silindi." });
        }
    }
}