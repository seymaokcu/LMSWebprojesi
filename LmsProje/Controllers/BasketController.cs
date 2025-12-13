using LmsProje.Data;
using LmsProje.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LmsProje.Controllers
{
    public class BasketController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var basket = await GetUserBasket(user.Id);
            return View(basket);
        }

        public async Task<IActionResult> AddToBasket(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = "/Home/Details/" + courseId });

            var basket = await GetUserBasket(user.Id);
            var course = _context.Courses.Find(courseId);

            if (!basket.Items.Any(x => x.CourseId == courseId))
            {
                basket.Items.Add(new BasketItem
                {
                    CourseId = courseId,
                    Price = course.Price
                });
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int itemId)
        {
            var item = _context.BasketItems.Find(itemId);
            if (item != null)
            {
                _context.BasketItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            var basket = await GetUserBasket(user.Id);

            if (!basket.Items.Any()) return RedirectToAction("Index");

            return View(basket);
        }

        [HttpPost]
        public async Task<IActionResult> CompletePayment()
        {
            var user = await _userManager.GetUserAsync(User);
            var basket = await GetUserBasket(user.Id);

            if (!basket.Items.Any()) return RedirectToAction("Index");

            var newOrder = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.Now,
                OrderStatus = "Completed",
                TotalPrice = basket.Items.Sum(x => x.Price)
            };

            foreach (var item in basket.Items)
            {
                newOrder.OrderItems.Add(new OrderItem
                {
                    CourseId = item.CourseId,
                    Price = item.Price
                });
            }

            _context.Orders.Add(newOrder);

            _context.BasketItems.RemoveRange(basket.Items);

            await _context.SaveChangesAsync();

            return RedirectToAction("Success", "Checkout", new { orderId = newOrder.Id });
        }
        private async Task<Basket> GetUserBasket(string userId)
        {
            var basket = await _context.Baskets
                .Include(b => b.Items).ThenInclude(i => i.Course)
                .FirstOrDefaultAsync(b => b.UserId == userId);

            if (basket == null)
            {
                basket = new Basket { UserId = userId };
                _context.Baskets.Add(basket);
                await _context.SaveChangesAsync();
            }
            return basket;
        }
    }
}
