using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using StajYonetimSistemi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StajYonetimSistemi.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var studentInfo = _context.StudentDatasets.FirstOrDefault(s => s.Eposta == email);
            var documents = _context.InternshipDocuments.Where(d => d.StudentEmail == email).ToList();

            ViewBag.StudentInfo = studentInfo;
            return View(documents);
        }

        [HttpPost]
        public IActionResult UploadDocument(DateTime startDate, DateTime endDate, bool workSaturday, IFormFile? documentFile)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var student = _context.StudentDatasets.FirstOrDefault(s => s.Eposta == email);
            string savedFilePath = "ornek_staj_evraki.pdf";

            if (documentFile != null && documentFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                savedFilePath = Path.GetFileName(documentFile.FileName);
                var fullPath = Path.Combine(uploadsFolder, savedFilePath);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    documentFile.CopyTo(stream);
                }
            }

            var newDoc = new InternshipDocument
            {
                StudentEmail = email,
                FilePath = savedFilePath,
                StartDate = startDate,
                EndDate = endDate,
                SubmissionDate = DateTime.Now,
                SelectedDays = workSaturday ? new List<string> { "Cumartesi" } : new List<string>()
            };

            // Örnek resmi tatiller listesi
            var publicHolidays = new List<DateTime>
            {
                new DateTime(DateTime.Now.Year, 1, 1),
                new DateTime(DateTime.Now.Year, 4, 23),
                new DateTime(DateTime.Now.Year, 5, 1),
                new DateTime(DateTime.Now.Year, 5, 19),
                new DateTime(DateTime.Now.Year, 7, 15),
                new DateTime(DateTime.Now.Year, 8, 30),
                new DateTime(DateTime.Now.Year, 10, 29)
            };

            // Yazdığımız tüm doğrulama kurallarını (TC, Adres, 30 gün, İş günü) çalıştırıyoruz
            var validationErrors = DocumentValidator.ValidateAll(newDoc, student!, publicHolidays, requiredDays: 20);

          

            if (validationErrors.Count > 0)
            {
                newDoc.Status = DocumentStatus.Reddedildi;
                TempData["Errors"] = string.Join(" | ", validationErrors);
            }
            else
            {
                newDoc.Status = DocumentStatus.Beklemede;
                TempData["Success"] = "Staj evrakınız tüm doğrulama kurallarından geçti ve onaylandı!";
            }

              _context.InternshipDocuments.Add(newDoc);
              _context.SaveChanges();
            
            return RedirectToAction("Index");
        }
    }
}