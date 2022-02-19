using MyBlog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Areas.Admin.Pages.Role
{
    public class RolePageModel : PageModel
    {
        protected readonly RoleManager<IdentityRole> _roleManager;

        protected readonly MyBlogContext _context;

        [TempData]
        public string StatusMessage { set; get; }

        public RolePageModel(RoleManager<IdentityRole> roleManager,MyBlogContext myBlogContext)
        {
            _roleManager = roleManager;
            _context = myBlogContext;
        }

    }
}
