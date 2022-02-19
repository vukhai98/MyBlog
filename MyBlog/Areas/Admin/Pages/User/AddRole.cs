using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyBlog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MyBlog.Areas.Admin.Pages.User
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly MyBlogContext _context;

        public AddRoleModel(UserManager<AppUser> userManager,
               SignInManager<AppUser> signInManager,
               RoleManager<IdentityRole> roleManager,
               MyBlogContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            keyValues = new List<KeyValuePair<string, string>>();
            _context = context;
        }

       

        [TempData]
        public string StatusMessage { get; set; }
       
        [BindProperty]
        [DisplayName("Các role gán cho user")]
        public string[] RoleNames { set; get; }

        public SelectList  allRoles { set; get; }

        public List<KeyValuePair<string,string>> keyValues { set; get; }
        public string  UserName { set; get; }

        public AppUser User { set; get; }

     
        public List<IdentityRoleClaim<string>> claimsInRole { set; get; }
        public List<IdentityUserClaim<string>> claimsInUserClaim { set; get; }

        public async Task GetClaims(string id)
        {
            var listRoles = from r in _context.Roles
                            join ur in _context.UserRoles on r.Id equals ur.RoleId
                            where ur.UserId == id
                            select r;
            var _claimInRole = from c in _context.RoleClaims
                               join r in listRoles on c.RoleId equals r.Id
                               select c;
            claimsInRole = await _claimInRole.ToListAsync();

            claimsInUserClaim = await (from c in _context.UserClaims
                                       where c.UserId == id
                                       select c).ToListAsync();
                            

        }


        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Không có user");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Không thấy user, id = {id}.");
            }
            UserName = user.UserName;
            User = user;
            RoleNames = (await _userManager.GetRolesAsync(user)).ToArray<string>();

            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles = new SelectList(roleNames);

            //List<IdentityRole> roleNames2 = await _roleManager.Roles.ToListAsync();
            //if(roleNames2 != null && roleNames2.Any())
            //{
            //    keyValues = roleNames2.Select((s, i) => new KeyValuePair<string, string>(s.Id, s.Name)).ToList();
            //}
            await GetClaims(id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Không có user");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Không thấy user, id = {id}.");
            }


            //Role Name

            await GetClaims(id);
            // Lấy tất cả các role của user đang có 
            var oldRoleNames = (await _userManager.GetRolesAsync(user)).ToArray();

            // xóa các role mà không có trong roleName
            var deleteRoles = oldRoleNames.Where(r => !RoleNames.Contains(r));

            // lấy các role mà có ở Role Name mà không cos trong oldRoleName
            var addRoles = RoleNames.Where(r => !oldRoleNames.Contains(r));


            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles = new SelectList(roleNames);

            var resultDelete = await _userManager.RemoveFromRolesAsync(user, deleteRoles);
            if (!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(error => {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();

            }

            var resultAdd = await _userManager.AddToRolesAsync(user, addRoles);
            if (!resultDelete.Succeeded)
            {
                resultAdd.Errors.ToList().ForEach(error => {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }


            StatusMessage = $"Vừa cập nhật role cho user:{user.UserName}.";

            return RedirectToPage("./Index");
        }
    }
}
