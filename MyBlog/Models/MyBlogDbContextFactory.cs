using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models
{
    public class MyBlogDbContextFactory : IDesignTimeDbContextFactory<MyBlogContext>
    {
        public MyBlogContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyBlogContext>();
            // pass your design time connection string here
            optionsBuilder.UseSqlServer("Server=ADMIN;Database=razorwebdb;Trusted_Connection=True;");
            return new MyBlogContext(optionsBuilder.Options);
        }
    }
}
