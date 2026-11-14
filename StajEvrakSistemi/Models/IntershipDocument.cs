using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace StajYonetimSistemi.Models
{
    public enum DocumentStatus
    {
        Beklemede,
        Onaylandi,
        Reddedildi
    }

    public class InternshipDocument
    {
        [Key]
        public int Id { get; set; }

        public string StudentEmail { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DocumentStatus Status { get; set; } = DocumentStatus.Beklemede;

        // Veritabanında liste hatası almamak için NotMapped veya JSON dönüşümü kullanılır
        [NotMapped]
        public List<string> SelectedDays { get; set; } = new List<string>();

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime SubmissionDate { get; set; } = DateTime.Now;

        [NotMapped]
        public List<string> ValidationErrors { get; set; } = new List<string>();
    }

    // Bütün validasyon kontrollerini topladığımız static sınıf
    public static class DocumentValidator
    {
        // 1. Adres içinde mahalle, cadde/sokak ve ilçe/il geçiyor mu kontrolü
        public static bool IsValidAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) return false;
            string lower = address.ToLower();
            bool hasMahalle = lower.Contains("mahalle") || lower.Contains("mah.") || lower.Contains("mh.");
            bool hasLocation = lower.Contains("/") || lower.Contains("ilçe") || lower.Contains("مرکز");
            return hasMahalle && address.Length >= 15 && (hasLocation || address.Split(' ').Length >= 3);
        }

        // 2. TC Kimlik (11 hane ve algoritma) veya Yabancı Kimlik / Pasaport kontrolü
        public static bool IsValidIdentity(string identityNo, string nationality)
        {
            if (string.IsNullOrWhiteSpace(identityNo)) return false;
            identityNo = identityNo.Trim();

            if (string.IsNullOrWhiteSpace(nationality) || nationality.ToUpper() == "TC" || nationality.ToLower().Contains("türk"))
            {
                // TC Kimlik Kuralları: 11 hane, rakam, 0 ile başlayamaz ve son hane çift olmalı
                if (identityNo.Length != 11 || !identityNo.All(char.IsDigit) || identityNo.StartsWith("0"))
                    return false;

                int lastDigit = int.Parse(identityNo[10].ToString());
                return lastDigit % 2 == 0;
            }
            else
            {
                // Yabancı uyruklu (99 ile başlayan YKN veya Pasaport numarası)
                return identityNo.Length >= 7 && identityNo.Length <= 11 && identityNo.All(char.IsLetterOrDigit);
            }
        }

        // 3. Staj bitiş tarihinden itibaren en geç 1 ay (30 gün) içinde teslim kuralı
        public static bool IsSubmittedOnTime(DateTime endDate, DateTime submissionDate)
        {
            if (submissionDate < endDate) return false;
            TimeSpan diff = submissionDate.Date - endDate.Date;
            return diff.TotalDays <= 30;
        }

        // 4. Resmi tatiller ve Pazar günleri hariç net çalışma günü hesaplama
        public static int CalculateValidWorkDays(DateTime start, DateTime end, List<DateTime> publicHolidays, bool canWorkSaturday = false)
        {
            if (end < start) return 0;

            int validDays = 0;
            for (DateTime date = start.Date; date <= end.Date; date = date.AddDays(1))
            {
                // Pazar günleri staj yapılamaz
                if (date.DayOfWeek == DayOfWeek.Sunday) continue;

                // Cumartesi çalışılabilir seçilmediyse Cumartesi günlerini atla
                if (date.DayOfWeek == DayOfWeek.Saturday && !canWorkSaturday) continue;

                // Resmi tatil günlerini atla
                if (publicHolidays != null && publicHolidays.Any(h => h.Date == date.Date)) continue;

                validDays++;
            }
            return validDays;
        }

        // 5. Öğrenci numarası ve E-posta format kontrolü
        public static bool IsValidStudentInfo(string ogrenciNo, string email)
        {
            bool validNo = !string.IsNullOrWhiteSpace(ogrenciNo) && ogrenciNo.Length >= 8 && ogrenciNo.All(char.IsDigit);
            bool validEmail = !string.IsNullOrWhiteSpace(email) && email.Contains("@") && email.Contains(".");
            return validNo && validEmail;
        }

        // 6. Tüm evrak ve öğrenci verilerini tek seferde denetleyip hataları döndüren ana metot
        public static List<string> ValidateAll(InternshipDocument doc, StudentDataset student, List<DateTime> holidays, int requiredDays = 20)
        {
            List<string> errors = new List<string>();

            if (student == null)
            {
                errors.Add("Öğrenci veri seti kaydı bulunamadı.");
                return errors;
            }

            if (!IsValidIdentity(student.TcKimlikNo, student.Uyruk))
                errors.Add("Kimlik veya Pasaport numarası formatı geçersiz.");

            if (!IsValidAddress(student.IkametgahAdresi))
                errors.Add("İkametgah adresi eksik: Mahalle ve ilçe/il bilgisi açıkça yazılmalıdır.");

            if (!IsValidStudentInfo(student.OgrenciNo, student.Eposta))
                errors.Add("Öğrenci numarası veya e-posta adresi hatalı.");

            if (!IsSubmittedOnTime(doc.EndDate, doc.SubmissionDate))
                errors.Add("Staj evrakı teslim süresi (bitiş tarihinden itibaren en fazla 30 gün) aşılmış.");

            bool worksSaturday = doc.SelectedDays != null && doc.SelectedDays.Contains("Cumartesi");
            int totalWorkDays = CalculateValidWorkDays(doc.StartDate, doc.EndDate, holidays, worksSaturday);

            if (totalWorkDays < requiredDays)
                errors.Add($"Geçerli iş günü sayısı yetersiz. Hesaplanan: {totalWorkDays} gün, Gereken en az: {requiredDays} gün.");

            if (string.IsNullOrWhiteSpace(student.SaglikYardimiStatusu))
                errors.Add("Sağlık yardımı (GSS/Aile) statüsü belirtilmemiş.");

            doc.ValidationErrors = errors;
            doc.Status = errors.Count == 0 ? DocumentStatus.Onaylandi : DocumentStatus.Reddedildi;

            return errors;
        }
    }
}