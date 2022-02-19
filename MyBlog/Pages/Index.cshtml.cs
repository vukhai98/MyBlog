using MyBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Pages
{
    //[AllowAnonymous]
    [Authorize] // Người dùng phải đăng nhập khi truy trập trang này 
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly MyBlogContext _myBlogContext;
        public IndexModel(ILogger<IndexModel> logger, MyBlogContext myBlogContext)
        {
            _logger = logger;
            _myBlogContext = myBlogContext;
        }

        public void OnGet()
        {
            var posts = (from p in _myBlogContext.Articles
                         orderby p.Created descending
                         select p).ToList();
            ViewData["post"] = posts;
        }
    }
}
