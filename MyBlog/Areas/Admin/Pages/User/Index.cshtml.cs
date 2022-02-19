using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace MyBlog.Areas.Admin.Pages.User
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public class UserAndRole : AppUser
        {
            public string RoleNames { set; get; }
        }

        public List<UserAndRole> users { set; get; }

        [TempData]
        public string StatusMessage { set; get; }

        public const int ITEMS_PER_PAGE = 10;
        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }
        public int countPages { get; set; }

        public int totalUser { set; get; }


        public async Task OnGet()
        {
            //users = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
            var qr = _userManager.Users.OrderBy(u => u.UserName);

             totalUser = await qr.CountAsync();

            // Tính số trang hiện thị (mỗi trang hiện thị ITEMS_PER_PAGE mục do bạn cấu hình = 10, 20 ...)
            countPages = (int)Math.Ceiling((double)totalUser / ITEMS_PER_PAGE);

            if (currentPage < 1)
            {
                currentPage = 1;
            }
            if (currentPage > countPages)
            {
                currentPage = countPages;
            }
            var qr1 = qr.Skip((currentPage - 1) * ITEMS_PER_PAGE)
                        .Take(ITEMS_PER_PAGE)
                        .Select(u => new UserAndRole { 
                            Id = u.Id,
                            UserName = u.UserName
                        });
            users = await qr1.ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join(",", roles);
            }
        }
        public void OnPost() => RedirectToPage();

    }
}
