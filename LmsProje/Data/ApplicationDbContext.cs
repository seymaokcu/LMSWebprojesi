using LmsProje.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;

namespace LmsProje.Data
{
    
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Entities.Section> Sections { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<UserLessonProgress> UserLessonProgresses { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DbSet<Favorite> Favorites { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Order>().Property(x => x.TotalPrice).HasColumnType("decimal(18,2)");
            builder.Entity<OrderItem>().Property(x => x.Price).HasColumnType("decimal(18,2)");
            builder.Entity<Course>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)");
        }

    }
}
