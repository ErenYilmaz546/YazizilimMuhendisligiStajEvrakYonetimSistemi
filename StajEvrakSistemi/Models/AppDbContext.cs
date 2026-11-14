using Microsoft.EntityFrameworkCore;
using StajYonetimSistemi.Models;

namespace StajYonetimSistemi.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<StudentDataset> StudentDatasets { get; set; }
        public DbSet<InternshipDocument> InternshipDocuments { get; set; }
    }
}