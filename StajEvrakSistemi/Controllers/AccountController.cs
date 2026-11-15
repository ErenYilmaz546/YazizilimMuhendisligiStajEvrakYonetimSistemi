using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using StajYonetimSistemi.Models;
using System.Linq;

namespace StajYonetimSistemi.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
            SeedInitialUsers();
        }

        // Veritabanı boşsa test için örnek Öğrenci, Admin ve Veri Seti kaydı oluşturur
        private void SeedInitialUsers()
        {
            if (!_context.Users.Any())
            {
                var defaultStudent = new Student
                {
                    Email = "erenyilmaz@kocaeli.edu.tr",
                    PasswordHash = "123456",
                    NameSurname = "Eren Yılmaz",
                    TcKimlikNo = "12345678902",
                    FailedLoginAttempts = 0
                };

                var defaultAdmin = new Admin
                {
                    Email = "admin@kocaeli.edu.tr",
                    PasswordHash = "admin123",
                    Department = "Yazılım Mühendisliği Staj Komisyonu",
                    FailedLoginAttempts = 0
                };

                _context.Students.Add(defaultStudent);
                _context.Admins.Add(defaultAdmin);

                if (!_context.StudentDatasets.Any())
                {
                    _context.StudentDatasets.Add(new StudentDataset
                    {
                        AdSoyad = "Eren Yılmaz",
                        TcKimlikNo = "12345678902",
                        Uyruk = "TC",
                        OgrenciNo = "220201001",
                        DogumTarihi = new System.DateTime(2004, 5, 10),
                        IkametgahAdresi = "Kabaoğlu Mah. Baki Komsuoğlu Bulvarı İzmit/Kocaeli",
                        Bolum = "Yazılım Mühendisliği",
                        TeslimEttigiYil = "2026",
                        DahaOnceYapilanStajGunSayisi = 0,
                        Eposta = "erenyilmaz@kocaeli.edu.tr",
                        Telefon = "05551234567",
                        SaglikYardimiStatusu = "Aileden (SGK)"
                    });
                }

                _context.SaveChanges();
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Lütfen e-posta ve şifre alanlarını doldurun.";
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email.Trim());

            if (user == null)
            {
                ViewBag.Error = "Kullanıcı bulunamadı.";
                return View();
            }

            // 5 başarısız giriş kontrolü
            if (user.IsLocked)
            {
                ViewBag.Error = "Hesabınız 5 hatalı giriş denemesi nedeniyle kilitlenmiştir! Lütfen sistem yöneticisiyle iletişime geçin.";
                return View();
            }

            // Şifre kontrolü
            if (user.PasswordHash != password)
            {
                user.FailedLoginAttempts++;
                _context.SaveChanges();

                int remaining = 5 - user.FailedLoginAttempts;
                if (user.IsLocked)
                {
                    ViewBag.Error = "Şifre hatalı! 5. başarısız deneme sonucunda hesabınız kilitlendi.";
                }
                else
                {
                    ViewBag.Error = $"Hatalı şifre! Kalan deneme hakkınız: {remaining}";
                }
                return View();
            }

            // Başarılı giriş: Hata sayacını sıfırla ve Session (Oturum) bilgilerini kaydet
            user.FailedLoginAttempts = 0;
            _context.SaveChanges();

            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.GetRoleType());

            // Polimorfizm (GetRoleType) ile rol bazlı yönlendirme
            if (user.GetRoleType() == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Student");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        [HttpPost]
        public IActionResult Register(string adSoyad, string ogrenciNo, string tcKimlikNo, string bolum, string ikametgah, string saglikStatusu, string email, string password)
        {
            if (_context.Users.Any(u => u.Email == email))
            {
                TempData["RegisterError"] = "Bu e-posta adresi zaten sistemde kayıtlı!";
                return RedirectToAction("Login");
            }

            // 1. Yeni Öğrenci Kullanıcısı (User -> Student) oluştur
            var newStudent = new Student
            {
                Email = email,
                PasswordHash = password,
                NameSurname = adSoyad,
                TcKimlikNo = tcKimlikNo,
                FailedLoginAttempts = 0
            };
            _context.Students.Add(newStudent);

            // 2. Evrak doğrulama sistemi için StudentDataset kaydını da otomatik oluştur
            var newDataset = new StudentDataset
            {
                AdSoyad = adSoyad,
                TcKimlikNo = tcKimlikNo,
                Uyruk = "TC",
                OgrenciNo = ogrenciNo,
                DogumTarihi = new DateTime(2004, 1, 1),
                IkametgahAdresi = ikametgah,
                Bolum = bolum,
                TeslimEttigiYil = "2026",
                DahaOnceYapilanStajGunSayisi = 0,
                Eposta = email,
                Telefon = "05550000000",
                SaglikYardimiStatusu = saglikStatusu
            };
            _context.StudentDatasets.Add(newDataset);
            _context.SaveChanges();

            TempData["RegisterSuccess"] = "Öğrenci kaydınız başarıyla oluşturuldu! Şimdi giriş yapabilirsiniz.";
            return RedirectToAction("Login");
          
       }
    }
}