using LmsProje.Data;
using LmsProje.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LmsProje.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var courses = _context.Courses.Include(c => c.Category).ToList();
            return View(courses);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Course course)
        {
            ModelState.Remove("Category");
            ModelState.Remove("Instructor");
            ModelState.Remove("Sections");

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(course.ImageUrl))
                    course.ImageUrl = "https://placehold.co/600x400";

                if (string.IsNullOrEmpty(course.Content))
                    course.Content = "Detaylı içerik girilmedi.";

                _context.Courses.Add(course);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            return View(course);
        }
        public IActionResult Delete(int id)
        {
            var course = _context.Courses.Find(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}