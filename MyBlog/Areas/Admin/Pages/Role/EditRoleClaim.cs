using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MyBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyBlog.Areas.Admin.Pages.Role
{
    [Authorize(Roles = "Admin")]
    public class EditRoleClaimModel : RolePageModel
    {
        public EditRoleClaimModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base(roleManager, myBlogContext)
        {
        }



        public class InputModel
        {
            [Display(Name = " Kiểu (tên) claim")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự. ")]
            public string ClaimType { set; get; }


            [Display(Name = " Giá trị")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự. ")]
            public string ClaimValue { set; get; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IdentityRole role { set; get; }

        IdentityRoleClaim<string> claim { set; get; }
        public async Task<IActionResult> OnGet(int? claimId)
        {
            if (claimId==null)
            {
                return NotFound("Không tìm thấy role");
            }
            claim = _context.RoleClaims.Where(c => c.Id == claimId).FirstOrDefault();
            if (claim==null)
            {
                return NotFound("Không tìm thấy role");
            }
            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role==null)
            {
                return NotFound("Không tìm thấy role");
                
            }
            Input = new InputModel()
            {
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue
            };

            return Page();
        }
        public async Task<IActionResult>OnPostAsync(int? claimId)
        {
            if (claimId == null)
            {
                return NotFound("Không tìm thấy role");
            }
            claim = _context.RoleClaims.Where(c => c.Id == claimId).FirstOrDefault();
            if (claim == null)
            {
                return NotFound("Không tìm thấy role");
            }
            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null)
            {
                return NotFound("Không tìm thấy role");

            }
            if (!ModelState.IsValid)
            {
                return Page();
            }




            if ( _context.RoleClaims.Any(c =>c.RoleId== role.Id && c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue && c.Id != claimId))
            {
                ModelState.AddModelError(string.Empty, "Claim này đã có trong role");
                return Page();
            }

            claim.ClaimType = Input.ClaimType;

            claim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();

            
            
            StatusMessage = " Vừa cập nhật claim";

            return RedirectToPage("./Edit", new { roleId = role.Id });



        }
        public async Task<IActionResult> OnPostDeleteAsync(int? claimId)
        {
            if (claimId == null)
            {
                return NotFound("Không tìm thấy role");
            }
            claim = _context.RoleClaims.Where(c => c.Id == claimId).FirstOrDefault();
            if (claim == null)
            {
                return NotFound("Không tìm thấy role");
            }
            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null)
            {
                return NotFound("Không tìm thấy role");

            }
            await _roleManager.RemoveClaimAsync(role, new Claim(claim.ClaimType, claim.ClaimValue));

            StatusMessage = " Vừa xóa claim";

            return RedirectToPage("./Edit", new { roleId = role.Id });



        }
    }
}
