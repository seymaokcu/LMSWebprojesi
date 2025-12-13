using System.ComponentModel.DataAnnotations;

namespace LmsProje.Entities
{
    public class Review
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; } // Yorumu yapan kişi

        public int CourseId { get; set; }
        public Course Course { get; set; } // Hangi kurs?

        [Range(1, 5)]
        public int Rating { get; set; } // 1 ile 5 arası puan

        public string Comment { get; set; } = string.Empty; // Yorum metni

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
