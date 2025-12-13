using LmsProje.Data;
using Microsoft.AspNetCore.Mvc;

namespace LmsProje.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CategoryMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            
            var categories = _context.Categories.OrderBy(x => x.Name).ToList();
            return View(categories);
        }
    }
}