using System.ComponentModel.DataAnnotations;

namespace LmsProje.Entities
{
    public class Lesson
    {
        public int Id { get; set; }
        [Required] public string Title { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }

        public int Duration { get; set; } 

        public bool IsPreview { get; set; } = false;

        public int SectionId { get; set; }
        public Section? Section { get; set; }
        public int DurationMinutes { get; set; }
    }
}