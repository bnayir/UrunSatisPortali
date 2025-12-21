using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

namespace UrunSatisPortali.Areas.Admin.Controllers
{
    [Area("Admin")] // BURASI ÇOK ÖNEMLİ!
    public class BrandController : Controller
    {
        private readonly IRepository<Brand> _brandRepo;

        public BrandController(IRepository<Brand> brandRepo)
        {
            _brandRepo = brandRepo;
        }

        public IActionResult Index()
        {
            var brands = _brandRepo.GetAll();
            return View(brands);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Brand brand)
        {
            if (ModelState.IsValid)
            {
                _brandRepo.Add(brand);
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }
        // BrandController.cs içine eklenecekler:

        public IActionResult Edit(int id)
        {
            var brand = _brandRepo.GetById(id);
            if (brand == null) return NotFound();
            return View(brand);
        }

        [HttpPost]
        public IActionResult Edit(Brand brand)
        {
            if (ModelState.IsValid)
            {
                _brandRepo.Update(brand);
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }
    }
}