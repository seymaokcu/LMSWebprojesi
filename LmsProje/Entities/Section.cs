using System.ComponentModel.DataAnnotations;

namespace LmsProje.Entities
{
    public class Section
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public List<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}