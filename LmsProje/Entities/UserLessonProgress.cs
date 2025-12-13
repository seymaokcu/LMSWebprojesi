using System.ComponentModel.DataAnnotations.Schema;

namespace LmsProje.Entities
{
    public class UserLessonProgress
    {
        public int Id { get; set; }

        public string UserId { get; set; } 
        public AppUser? User { get; set; }

        public int LessonId { get; set; } 
        public Lesson? Lesson { get; set; }

        public bool IsCompleted { get; set; } 
        public DateTime CompletedDate { get; set; } = DateTime.Now;
    }
}
