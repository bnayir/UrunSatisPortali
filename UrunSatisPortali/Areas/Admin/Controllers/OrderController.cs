using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrunSatisPortali.Data;
using UrunSatisPortali.Models;

namespace UrunSatisPortali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Sadece adminlerin girmesini sağlar
    public class OrderController : Controller
    {
        private readonly IRepository<Order> _orderRepo;

        public OrderController(IRepository<Order> orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public IActionResult Index()
        {
            // Veritabanındaki tüm siparişleri, en yeni en üstte olacak şekilde çeker
            var allOrders = _orderRepo.GetAll().OrderByDescending(x => x.OrderDate).ToList();
            return View(allOrders);
        }

        public IActionResult Details(int id)
        {
            var order = _orderRepo.GetById(id);
            if (order == null) return NotFound();
            return View(order);
        }
    }
}
