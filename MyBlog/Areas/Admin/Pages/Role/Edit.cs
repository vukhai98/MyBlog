using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MyBlog.Areas.Admin.Pages.Role
{
    // Policy: Tạo ra các policy --> AllowEditRole  
    [Authorize(Policy = "AllowEditRole")]
    public class EditModel : RolePageModel
    {
       
        public EditModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base(roleManager, myBlogContext)
        {
        }



        public class InputModel
        {
            [Display(Name = " Tên của role")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự. ")]
            public string Name { set; get; }
           
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<IdentityRoleClaim<string>> Claims { set; get; }
        public IdentityRole role { set; get; }
        public async Task<IActionResult> OnGet(string roleId)
        {
            if (roleId == null) 
            {
                return NotFound("Không tìm thấy role");                           
            }
            role = await _roleManager.FindByIdAsync(roleId);
            if (role != null)
            {
                Input = new InputModel()
                {
                    Name = role.Name
                };
                Claims = await _context.RoleClaims.Where(rc => rc.RoleId == role.Id).ToListAsync();
                return Page();
            }

            return NotFound("Không tìm thấy role");
          

        }
        public async Task<IActionResult>OnPostAsync(string roleId)
        {
            if (roleId == null) return NotFound("Không tìm thấy role");
            role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return NotFound("Không tìm thấy role");
            Claims = await _context.RoleClaims.Where(rc => rc.RoleId == role.Id).ToListAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }
            role.Name = Input.Name;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                StatusMessage = $" Bạn vừa đổi tên : {Input.Name}";
                return RedirectToPage("./Index");
            }
            else
            {
                result.Errors.ToList().ForEach(error => {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }
            return Page();
        }
    }
}
