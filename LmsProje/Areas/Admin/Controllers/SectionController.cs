using LmsProje.Data;
using LmsProje.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LmsProje.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SectionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SectionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Manage(int courseId)
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
            return RedirectToAction("Manage", new { courseId = courseId });
        }
        [HttpPost]
        public IActionResult AddLesson(int sectionId, string lessonTitle, string videoUrl, double duration)
        {
            var section = _context.Sections.Find(sectionId);
            if (section != null)
            {
                var lesson = new Lesson
                {
                    Title = lessonTitle,
                    VideoUrl = videoUrl,
                    Duration = (int)duration,
                    SectionId = sectionId,
                    IsPreview = false
                };
                _context.Lessons.Add(lesson);
                _context.SaveChanges();
                return RedirectToAction("Manage", new { courseId = section.CourseId });
            }
            return RedirectToAction("Index", "Course");
        }

        public IActionResult DeleteSection(int id)
        {
            var sec = _context.Sections.Find(id);
            if (sec != null)
            {
                int cid = sec.CourseId;
                _context.Sections.Remove(sec);
                _context.SaveChanges();
                return RedirectToAction("Manage", new { courseId = cid });
            }
            return RedirectToAction("Index", "Course");
        }

        public IActionResult DeleteLesson(int id)
        {
            var les = _context.Lessons.Include(l => l.Section).FirstOrDefault(x => x.Id == id);
            if (les != null)
            {
                int cid = les.Section?.CourseId ?? 0;
                _context.Lessons.Remove(les);
                _context.SaveChanges();
                if (cid != 0)
                    return RedirectToAction("Manage", new { courseId = cid });
            }
            return RedirectToAction("Index", "Course");
        }
    }
}