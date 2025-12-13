using LmsProje.Data;
using LmsProje.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LmsProje.Controllers
{
    public class InstructorController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context; 

        public InstructorController(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AppUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context; 
        }

        public IActionResult Index() { return View(); }

        [HttpPost]
        public async Task<IActionResult> BecomeInstructor()
        {
            var user = await _userManager.GetUserAsync(User);
            if (!await _roleManager.RoleExistsAsync("Teacher")) await _roleManager.CreateAsync(new IdentityRole("Teacher"));

            if (!await _userManager.IsInRoleAsync(user, "Teacher"))
            {
                await _userManager.AddToRoleAsync(user, "Teacher");
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Welcome");
        }

        public IActionResult Welcome() { return View(); }

        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> MyCourses()
        {
            var user = await _userManager.GetUserAsync(User);
            var courses = _context.Courses
                .Include(c => c.Category).Include(c => c.Reviews)
                .Where(c => c.InstructorId == user.Id).ToList();
            return View(courses);
        }

        [Authorize(Roles = "Teacher,Admin")]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [Authorize(Roles = "Teacher,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Course course, IFormFile? imageFile)
        {
            var user = await _userManager.GetUserAsync(User);
            course.InstructorId = user.Id;
            course.CreatedDate = DateTime.Now;

            if (imageFile != null)
            {
                var ext = Path.GetExtension(imageFile.FileName);
                var newName = "course_" + Guid.NewGuid() + ext;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/courses/", newName);
                using (var stream = new FileStream(path, FileMode.Create)) await imageFile.CopyToAsync(stream);
                course.ImageUrl = "/img/courses/" + newName;
            }
            else course.ImageUrl = "https://via.placeholder.com/600x400";

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return RedirectToAction("MyCourses");
        }
    }
}