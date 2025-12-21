using Microsoft.AspNetCore.Mvc;

namespace UrunSatisPortali.Areas.Admin.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
