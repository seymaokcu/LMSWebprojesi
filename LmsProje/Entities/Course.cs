using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LmsProje.Entities
{
    public class Course
    {
        public int Id { get; set; }
        [Required] public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Content { get; set; } 
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = "";
        public bool IsFree { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public string? InstructorId { get; set; }
        [ForeignKey("InstructorId")]
        public AppUser? Instructor { get; set; }

        public List<Section> Sections { get; set; } = new List<Section>();
        public List<Review> Reviews { get; set; } = new List<Review>();
        public List<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}