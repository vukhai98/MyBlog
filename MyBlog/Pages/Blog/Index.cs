using MyBlog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Pages.Blog
{

    public class IndexModel : PageModel
    {
        private readonly MyBlogContext _context;

        public IndexModel(MyBlogContext context)
        {
            _context = context;
            Article = new List<Article>();
        }

        public const int ITEMS_PER_PAGE = 10;
        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }
        public int countPages { get; set; }


        public IList<Article> Article { get; set; }

        // Chuỗi để tìm kiếm, được binding tự động kể cả là truy 
        // cập get
        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }
        public async Task OnGetAsync()
        {
            if (currentPage < 1)
            {
                currentPage = 1;
            }
            var query = _context.Articles.AsQueryable();

            if (!string.IsNullOrEmpty(Keyword))

            {
                // Truy vấn lọc các Article mà tiêu đề chứa chuỗi tìm kiếm
                query = query.Where(a => a.Title.Contains(Keyword));
            }

            var totalArticle = await query.CountAsync();

            // Tính số trang hiện thị (mỗi trang hiện thị ITEMS_PER_PAGE mục do bạn cấu hình = 10, 20 ...)
            countPages = (int)Math.Ceiling((double)totalArticle / ITEMS_PER_PAGE);

            if (currentPage > countPages)
            {
                currentPage = countPages;
            }

            if(totalArticle > 0)
            {
                // Truy vấn lấy các Article
                query = query.OrderByDescending(a => a.Created).Skip((currentPage - 1) * 10)
                             .Take(ITEMS_PER_PAGE);

                // Đọc (nạp) Article
                Article = await query.ToListAsync();
            }
        }

    }
}
