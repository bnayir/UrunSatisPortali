using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using UrunSatisPortali.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. VERÝTABANI BAÐLANTISI
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. KENDÝ ÖZEL GÝRÝÞ SÝSTEMÝN (Cookie Authentication)
// Senin AccountController içindeki "MyCookieAuth" ismiyle birebir ayný olmalý
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Account/Login"; // Senin Controller'ýnýn yolu
        options.AccessDeniedPath = "/Home/AccessDenied";
    });

// 3. SERVÝS KAYITLARI
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 4. HTTP PIPELINE AYARLARI
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Önce kimlik doðrulama, sonra yetkilendirme
app.UseAuthentication();
app.UseAuthorization();

// 5. ROTALAR
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();