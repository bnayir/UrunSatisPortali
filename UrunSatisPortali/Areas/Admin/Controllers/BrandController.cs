using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

namespace UrunSatisPortali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] 
    public class BrandController : Controller
    {
        private readonly IRepository<Brand> _brandRepo;

        public BrandController(IRepository<Brand> brandRepo)
        {
            _brandRepo = brandRepo;
        }

        // Listeleme
        public IActionResult Index()
        {
            var brands = _brandRepo.GetAll();
            return View(brands);
        }

        // Yeni Marka Ekleme (Sayfa)
        public IActionResult Create() => View();

        // Yeni Marka Ekleme (İşlem)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Brand brand)
        {
            if (ModelState.IsValid)
            {
                _brandRepo.Add(brand);
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        // Düzenleme (Sayfa)
        public IActionResult Edit(int id)
        {
            var brand = _brandRepo.GetById(id);
            if (brand == null) return NotFound();
            return View(brand);
        }

        // Düzenleme (İşlem)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Brand brand)
        {
            if (ModelState.IsValid)
            {
                _brandRepo.Update(brand);
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        // Silme İşlemi (Yeni Eklendi)
        public IActionResult Delete(int id)
        {
            var brand = _brandRepo.GetById(id);
            if (brand != null)
            {
                _brandRepo.Delete(brand);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}