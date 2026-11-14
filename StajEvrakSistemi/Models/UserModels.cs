using System.ComponentModel.DataAnnotations;

namespace StajYonetimSistemi.Models
{
    public abstract class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public int FailedLoginAttempts { get; set; } = 0;

        // Güvenlik kuralı: 5 başarısız denemede hesap kilitlenir
        public bool IsLocked
        {
            get { return FailedLoginAttempts >= 5; }
        }

        // Alt sınıfların kendine göre dolduracağı rol metodu (Polimorfizm)
        public abstract string GetRoleType();
    }

    // Öğrenci sınıfı (Kalıtım - Inheritance)
    public class Student : User
    {
        public string TcKimlikNo { get; set; } = string.Empty;
        public string NameSurname { get; set; } = string.Empty;

        public override string GetRoleType()
        {
            return "Student";
        }
    }

    // Yönetici / Komisyon sınıfı (Kalıtım - Inheritance)
    public class Admin : User
    {
        public string Department { get; set; } = string.Empty;

        public override string GetRoleType()
        {
            return "Admin";
        }
    }
}