using LmsProje.Data;
using LmsProje.Entities;
using LmsProje.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LmsProje.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var model = new DashboardViewModel();
            model.TotalCourses = _context.Courses.Count();
            model.TotalUsers = _context.Users.Count();
            model.TotalCategories = _context.Categories.Count();

            
            model.TotalPotentialRevenue = _context.Courses.Sum(x => x.Price);

            var categoryStats = _context.Courses
    .Include(c => c.Category)
    .GroupBy(x => x.Category != null ? x.Category.Name : "Genel")
    .Select(g => new
    {
        CategoryName = g.Key,
        Count = g.Count()
    })
    .ToList();

            foreach (var item in categoryStats)
            {
                model.CategoryNames.Add(item.CategoryName ?? "Genel");
                model.CourseCounts.Add(item.Count);
            }

            return View(model);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Category()
        {
         
            var categories = _context.Categories.ToList();
            return View(categories);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddCategory(string name, string url) // <-- Parametre ismi 'url' olmalı
        {
            if (!string.IsNullOrEmpty(name))
            {
               
                _context.Categories.Add(new Category { Name = name, Url = url });
                _context.SaveChanges();
            }
            return RedirectToAction("Category"); 
        }
    }
}
