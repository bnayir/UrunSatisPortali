using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class OrderController : Controller
{
    public IActionResult Index()
    {
        // Burada gerçekte veritabanındaki Siparişler tablosu listelenir
        return View();
    }
}