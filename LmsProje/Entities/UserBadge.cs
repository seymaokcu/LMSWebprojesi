namespace LmsProje.Entities
{
    public class UserBadge
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        public int BadgeId { get; set; }
        public Badge Badge { get; set; }

        public DateTime EarnedDate { get; set; } = DateTime.Now;
    }
}
