using LmsProje.Data;
using LmsProje.Entities;
using LmsProje.Models;
using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LmsProje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;


        public HomeController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var courses = _context.Courses
    .Include(c => c.Category)
    .Include(c => c.Reviews)
    .Include(c => c.Favorites)
                .ToList();

            return View(courses);
        }
        public IActionResult Details(int id)
        {
            var course = _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Sections).ThenInclude(s => s.Lessons)
                .Include(c => c.Reviews).ThenInclude(r => r.User)
                .FirstOrDefault(x => x.Id == id);

            if (course == null) return NotFound();

            return View(course);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Watch(int lessonId)
        {
            var activeLesson = _context.Lessons
                .Include(l => l.Section)
                .FirstOrDefault(l => l.Id == lessonId);

            if (activeLesson == null) return NotFound();

            var course = _context.Courses
                .Include(c => c.Sections).ThenInclude(s => s.Lessons)
                .FirstOrDefault(c => c.Id == activeLesson.Section.CourseId);

            ViewBag.CurrentLesson = activeLesson;

            bool isCompleted = false;
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                isCompleted = _context.UserLessonProgresses
                    .Any(x => x.UserId == userId && x.LessonId == lessonId && x.IsCompleted);
            }
            ViewBag.IsLessonCompleted = isCompleted;

            return View(course);
        }
        [HttpPost]
        public async Task<IActionResult> CompleteLesson(int lessonId)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");

            var userId = _userManager.GetUserId(User);

            var existingProgress = _context.UserLessonProgresses
                .FirstOrDefault(x => x.UserId == userId && x.LessonId == lessonId);

            if (existingProgress == null)
            {
                var progress = new UserLessonProgress { UserId = userId, LessonId = lessonId, IsCompleted = true };
                _context.UserLessonProgresses.Add(progress);
                await _context.SaveChangesAsync();
                await CheckAndAwardBadges(userId);
            }

            return RedirectToAction("Watch", new { lessonId = lessonId });
        }

        private async Task CheckAndAwardBadges(string userId)
        {
            int completedCount = _context.UserLessonProgresses.Count(x => x.UserId == userId && x.IsCompleted);

            if (completedCount == 1)
            {
                await AssignBadge(userId, "Ýlk Adým", "fa-solid fa-shoe-prints");
            }

            if (completedCount == 5)
            {
                await AssignBadge(userId, "Çalýþkan Arý", "fa-solid fa-bee");
            }
        }
        private async Task AssignBadge(string userId, string badgeName, string iconClass)
        {

            var badge = _context.Badges.FirstOrDefault(b => b.Name == badgeName);
            if (badge == null)
            {
                badge = new Badge { Name = badgeName, Description = "Baþarý kilidi açýldý!", IconUrl = iconClass };
                _context.Badges.Add(badge);
                await _context.SaveChangesAsync();
            }

            bool hasBadge = _context.UserBadges.Any(ub => ub.UserId == userId && ub.BadgeId == badge.Id);
            if (!hasBadge)
            {
                _context.UserBadges.Add(new UserBadge { UserId = userId, BadgeId = badge.Id });
                await _context.SaveChangesAsync();
            }
        }
        public IActionResult Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login", "Account");

            var myBadges = _context.UserBadges
                .Where(ub => ub.UserId == userId)
                .Include(ub => ub.Badge)
                .ToList();

            var myCourses = _context.UserLessonProgresses
                .Where(ulp => ulp.UserId == userId)
                .Include(ulp => ulp.Lesson).ThenInclude(l => l.Section).ThenInclude(s => s.Course)
                .Select(ulp => ulp.Lesson.Section.Course)
                .Distinct()
                .ToList();

            ViewBag.MyBadges = myBadges;
            return View(myCourses);
        }
    
    [HttpPost]
        public IActionResult AddReview(int courseId, int rating, string comment)
        {

            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");

            var userId = _userManager.GetUserId(User);

            var review = new Review
            {
                CourseId = courseId,
                UserId = userId,
                Rating = rating,
                Comment = comment,
                CreatedDate = DateTime.Now
            };

            _context.Reviews.Add(review);
            _context.SaveChanges();


            return RedirectToAction("Details", new { id = courseId });
        }
        
        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query)) return RedirectToAction("Index");

            
            var courses = _context.Courses
                .Include(c => c.Category)
                .Where(x => x.Title.Contains(query) || x.Description.Contains(query))
                .ToList();

            
            ViewBag.SearchQuery = query; 
            return View("Index", courses);
        }

        public async Task<IActionResult> FixAdminRole()
        {
            
            var roleManager = HttpContext.RequestServices.GetService<RoleManager<IdentityRole>>();
            var userManager = HttpContext.RequestServices.GetService<UserManager<AppUser>>();

            
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            
            var user = await userManager.FindByEmailAsync("admin@lms.com");
            if (user == null) return Content("HATA: admin@lms.com kullanýcýsý bulunamadý! Önce kayýt olmalý.");

            
            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.AddToRoleAsync(user, "Admin");
                return Content("BAÞARILI! Kullanýcýya 'Admin' yetkisi verildi. ÞÝMDÝ ÇIKIÞ YAPIP TEKRAR GÝRMELÝSÝN.");
            }

            return Content("Zaten yetkisi var. Sorun yok.");
        }


        public IActionResult Explore(string search, int? categoryId, string price, string sort)
        {
           
            var query = _context.Courses
                .Include(x => x.Category)
                .Include(x => x.Reviews)
                .Include(x => x.Favorites) 
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Title.Contains(search) || x.Description.Contains(search));
                ViewBag.Search = search; 
            }

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
                ViewBag.CategoryId = categoryId;
            }

            
            if (!string.IsNullOrEmpty(price))
            {
                if (price == "free") query = query.Where(x => x.IsFree);
                else if (price == "paid") query = query.Where(x => !x.IsFree);

                ViewBag.Price = price;
            }

            switch (sort)
            {
                case "price_asc": 
                    query = query.OrderBy(x => x.Price);
                    break;
                case "price_desc": 
                    query = query.OrderByDescending(x => x.Price);
                    break;
                case "newest": 
                    query = query.OrderByDescending(x => x.CreatedDate);
                    break;
                default: 
                    query = query.OrderByDescending(x => x.Id);
                    break;
            }
            ViewBag.Sort = sort;

            var result = query.ToList();

            ViewBag.Categories = _context.Categories.ToList();

            return View(result);
        }
    } 


}