using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MyBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace MyBlog.Areas.Admin.Pages.Role
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : RolePageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IndexModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext, IHttpContextAccessor httpContextAccessor) : base(roleManager, myBlogContext)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public class RoleModel: IdentityRole
        {
            public string[] Claims { get; set; }
        }
        public List<RoleModel> roles { set; get; }

        public async Task<IActionResult> OnGet()
        {
           // var claims = _httpContextAccessor.HttpContext.User.Claims;//data in claim
            var r =  await _roleManager.Roles.OrderBy(r =>r.Name).ToListAsync();
            roles = new List<RoleModel>();
            foreach (var _r in r)
            {
                var claims = await _roleManager.GetClaimsAsync(_r);
                var claimString = claims.Select(c => c.Type + "=" + c.Value);

                var rm = new RoleModel()
                {
                    Name = _r.Name,
                    Id = _r.Id,
                    Claims = claimString.ToArray()
                };
                roles.Add(rm);
            }
            return Page();
        }
        

    }
}
