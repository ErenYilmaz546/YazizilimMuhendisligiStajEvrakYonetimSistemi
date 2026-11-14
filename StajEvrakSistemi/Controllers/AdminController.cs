using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using StajYonetimSistemi.Models;
using System.Linq;

namespace StajYonetimSistemi.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var allDocuments = _context.InternshipDocuments.ToList();
            ViewBag.Students = _context.StudentDatasets.ToList();

            return View(allDocuments);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, DocumentStatus status)
        {
            var doc = _context.InternshipDocuments.FirstOrDefault(d => d.Id == id);
            if (doc != null)
            {
                doc.Status = status;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}