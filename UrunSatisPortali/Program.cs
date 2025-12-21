using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrunSatisPortali.Data;
using UrunSatisPortali.Hubs;

var builder = WebApplication.CreateBuilder(args);

// 1. VERİTABANI BAĞLANTISI
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. IDENTITY SERVİSLERİ
// NOT: .AddDefaultIdentity yerine .AddIdentity kullanmak Rol yönetimi (Admin) için bazen daha kararlıdır.
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI(); // Identity ekranlarının (Login/Register) çalışması için şart.

// 3. SERVİS KAYITLARI
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSignalR(); // SignalR servisi eklendi.

var app = builder.Build();

// 4. HTTP PIPELINE
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// SIRALAMA: Authentication her zaman Authorization'dan önce gelmeli!
app.UseAuthentication();
app.UseAuthorization();

// 5. SIGNALR HUB EŞLEŞTİRMESİ
// Buradaki "/dashboardHub" ismi JS tarafındaki withUrl("/dashboardHub") ile aynı olmalı.
app.MapHub<DashboardHub>("/dashboardHub");

// 6. ROTALAR
app.MapRazorPages();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();