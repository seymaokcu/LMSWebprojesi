using LmsProje.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LmsProje.ViewComponents
{
    public class UserAvatarViewComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;

        public UserAvatarViewComponent(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                return View("Default", user);
            }
            return View("Default", new AppUser());
        }
    }
}