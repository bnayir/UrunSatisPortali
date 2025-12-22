using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UrunSatisPortali.Controllers
{
    public class AccountController : Controller
    {
        // Identity sisteminin asıl yöneticisi SignInManager'dır
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public IActionResult Login(string returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = "/")
        {
            // Identity sistemi üzerinden giriş kontrolü
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            ViewData["LoginError"] = "Kullanıcı adı veya şifre yanlış.";
            return View();
        }

        // BU KISIM HATAYI ÇÖZER: MyCookieAuth yerine Identity metodunu kullanıyoruz
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}