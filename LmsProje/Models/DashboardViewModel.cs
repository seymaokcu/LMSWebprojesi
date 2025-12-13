namespace LmsProje.Models
{
    public class DashboardViewModel
    {
        public int TotalCourses { get; set; }
        public int TotalUsers { get; set; }
        public int TotalCategories { get; set; }
        public decimal TotalPotentialRevenue { get; set; } // Tüm kursların toplam fiyatı

        public List<string> CategoryNames { get; set; } = new List<string>(); // Kategori İsimleri (X Ekseni)
        public List<int> CourseCounts { get; set; } = new List<int>();       // Kategori Başına Kurs Sayısı (Y Ekseni)
    }
}
