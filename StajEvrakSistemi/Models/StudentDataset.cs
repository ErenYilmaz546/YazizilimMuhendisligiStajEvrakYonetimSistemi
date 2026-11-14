using System.ComponentModel.DataAnnotations;

namespace StajYonetimSistemi.Models
{
    public class StudentDataset
    {
        [Key]
        public int Id { get; set; }
        public string AdSoyad { get; set; } = string.Empty;
        public string TcKimlikNo { get; set; } = string.Empty;
        public string Uyruk { get; set; } = string.Empty;
        public string OgrenciNo { get; set; } = string.Empty;
        public DateTime DogumTarihi { get; set; }
        public string IkametgahAdresi { get; set; } = string.Empty;
        public string Bolum { get; set; } = string.Empty;
        public string TeslimEttigiYil { get; set; } = string.Empty;
        public int DahaOnceYapilanStajGunSayisi { get; set; }
        public string Eposta { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string SaglikYardimiStatusu { get; set; } = string.Empty;
    }
}