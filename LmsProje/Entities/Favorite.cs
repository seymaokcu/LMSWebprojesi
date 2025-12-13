namespace LmsProje.Entities
{
    public class Favorite
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public AppUser? User { get; set; }

        public int CourseId { get; set; }
        public Course? Course { get; set; }
    }
}