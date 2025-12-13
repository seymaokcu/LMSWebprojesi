using System.ComponentModel.DataAnnotations;

namespace LmsProje.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Url { get; set; }

        public List<Course> Courses { get; set; } = new List<Course>();
    }
}