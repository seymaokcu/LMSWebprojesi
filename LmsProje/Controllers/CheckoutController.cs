using LmsProje.Data;
using LmsProje.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LmsProje.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CheckoutController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> BuyCourse(int courseId)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");

            var user = await _userManager.GetUserAsync(User);
            var course = _context.Courses.Find(courseId);

            if (course == null) return NotFound();

            bool alreadyBought = _context.OrderItems.Any(x => x.Order.UserId == user.Id && x.CourseId == courseId);
            if (alreadyBought)
            {
                TempData["Message"] = "Bu kursa zaten sahipsiniz.";
                return RedirectToAction("Dashboard", "Home");
            }

            var newOrder = new Order
            {
                UserId = user.Id,
                TotalPrice = course.Price,
                OrderDate = DateTime.Now,
                OrderStatus = "Completed"
            };

            var orderItem = new OrderItem
            {
                CourseId = courseId,
                Price = course.Price,
                Order = newOrder 
            };

            _context.Orders.Add(newOrder); 
            await _context.SaveChangesAsync();

            return RedirectToAction("Success", new { orderId = newOrder.Id });
        }


        public IActionResult Success(int orderId)
        {
            return View(orderId);
        }
    }
}
