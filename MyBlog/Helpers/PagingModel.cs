using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Helpers
{
    public class PagingModel
    {
        public int currentPage { get; set; }
        public int countPages { set; get; }
        public string Keyword { set; get; }
        public Func<int?, string> generateUrl { set; get; }
        public bool IsActive => string.IsNullOrWhiteSpace(Keyword) ? false : true;
    }
}
