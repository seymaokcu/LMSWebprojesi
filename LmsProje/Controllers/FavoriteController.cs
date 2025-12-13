using LmsProje.Data;
using LmsProje.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LmsProje.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public FavoriteController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var favorites = _context.Favorites
                .Include(f => f.Course)
                .Where(f => f.UserId == user.Id)
                .ToList();

            return View(favorites);
        }
        [HttpPost]
        public async Task<IActionResult> Toggle(int courseId, string returnUrl)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account", new { returnUrl = returnUrl });

            var user = await _userManager.GetUserAsync(User);

            var existingFav = _context.Favorites
                .FirstOrDefault(f => f.UserId == user.Id && f.CourseId == courseId);

            if (existingFav != null)
            {
                _context.Favorites.Remove(existingFav);
            }
            else
            {
                _context.Favorites.Add(new Favorite { UserId = user.Id, CourseId = courseId });
            }

            await _context.SaveChangesAsync();

            return Redirect(returnUrl);
        }
    }
}