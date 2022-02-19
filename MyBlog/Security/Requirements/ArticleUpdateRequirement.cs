using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyBlog.Security.Requirements
{
    public class ArticleUpdateRequirement : IAuthorizationRequirement

    {
        public ArticleUpdateRequirement(int year = 2022, int month = 6, int date =30)
        {
            Year = year;
            Month = month;
            Date = date;
        }
        public int Year { set; get; }

        public int Month { set; get; }

        public int Date { set; get; }



    }
}
