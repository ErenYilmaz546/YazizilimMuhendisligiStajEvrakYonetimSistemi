using Microsoft.EntityFrameworkCore;
using StajYonetimSistemi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabanı (SQLite) bağlantısı servis kaydı
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Giriş yapan kullanıcıyı (Öğrenci/Admin) tutmak için Session (Oturum) servisi
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// MVC Servisi
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStatusCodePages("text/plain; charset=utf-8", "HTTP Durum Kodu: {0} - Aradığınız sayfa bulunamadı (404) veya bu sayfaya erişim yetkiniz yok (401).");
app.UseRouting();

// Oturum (Session) kullanımı aktif ediliyor
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();