using LmsProje.Data;
using LmsProje.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LmsProje.Controllers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class CourseManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int courseId)
        {
            var course = _context.Courses
                .Include(c => c.Sections)
                .ThenInclude(s => s.Lessons)
                .FirstOrDefault(c => c.Id == courseId);

            if (course == null) return NotFound();

            return View(course);
        }

        [HttpPost]
        public IActionResult AddSection(int courseId, string sectionName)
        {
            if (!string.IsNullOrEmpty(sectionName))
            {
                var section = new Section { Name = sectionName, CourseId = courseId };
                _context.Sections.Add(section);
                _context.SaveChanges();
            }
            return RedirectToAction("Index", new { courseId = courseId });
        }


        [HttpPost]
        public IActionResult AddLesson(int sectionId, string title, string videoUrl, int duration, bool isPreview)
        {
            var section = _context.Sections.Find(sectionId);
            if (section != null)
            {
                var lesson = new Lesson
                {
                    Title = title,
                    VideoUrl = videoUrl, // Örn: https://www.youtube.com/embed/VIDEO_ID
                    DurationMinutes = duration,
                    IsPreview = isPreview,
                    SectionId = sectionId
                };
                _context.Lessons.Add(lesson);
                _context.SaveChanges();

                
                return RedirectToAction("Index", new { courseId = section.CourseId });
            }
            return NotFound();
        }

        public IActionResult DeleteSection(int id)
        {
            var section = _context.Sections.Find(id);
            if (section != null)
            {
                int courseId = section.CourseId;
                _context.Sections.Remove(section); 
                _context.SaveChanges();
                return RedirectToAction("Index", new { courseId = courseId });
            }
            return NotFound();
        }
    }
}
