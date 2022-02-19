using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MyBlog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyBlog.Areas.Admin.Pages.User
{
    public class EditUserRoleClaimModel : PageModel
    {
        private readonly MyBlogContext _context;

        private readonly UserManager<AppUser> _userManager;
        public EditUserRoleClaimModel(MyBlogContext myBlogContext, UserManager<AppUser> userManager)
        {
            _context = myBlogContext;
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { set; get; }

        public NotFoundObjectResult OnGet()
        {
            return NotFound("Không truy cập được");
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

        public AppUser user { set; get; }

        public IdentityUserClaim<string> UserClaim { set; get; }

        public async Task<IActionResult> OnGetAddClaimAsync(string userId)
        {
            user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Không tìm thấy user");
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAddClaimAsync(string userId)
        {
            user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Không tìm thấy user");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var claims = _context.UserClaims.Where(c => c.UserId == user.Id);
            if (claims.Any(c => c.ClaimType == Input.ClaimType
                          && c.ClaimValue == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Đặc tính này đã có");
                return Page();
            }

            await _userManager.AddClaimAsync(user, new Claim(Input.ClaimType, Input.ClaimValue));

            StatusMessage = "Đã thêm dặc tính cho user";


            return RedirectToPage("./AddRole", new { id = user.Id });
        }
        public async Task<IActionResult> OnGetEditClaimAsync(int? claimId)
        {
            if (claimId == null) return NotFound("Không tìm thấy user");

            UserClaim = _context.UserClaims.Where(c => c.Id == claimId).FirstOrDefault();

            user = await _userManager.FindByIdAsync(UserClaim.UserId);

            if (user == null)
            {
                return NotFound("Không tìm thấy user");
            }

            Input = new InputModel()
            {
                ClaimType = UserClaim.ClaimType,
                ClaimValue = UserClaim.ClaimValue
            };

            return Page();
        }
        public async Task<IActionResult> OnPostEditClaimAsync(int? claimId)
        {
            if (claimId == null) return NotFound("Không tìm thấy user");

            UserClaim = _context.UserClaims.Where(c => c.Id == claimId).FirstOrDefault();

            user = await _userManager.FindByIdAsync(UserClaim.UserId);
            if (user == null)
            {
                return NotFound("Không tìm thấy user");
            }

            if (!ModelState.IsValid) return Page();

            if (
                    _context.UserClaims.Any(c => c.UserId == user.Id
                                             && c.ClaimType == Input.ClaimType
                                             && c.ClaimValue == Input.ClaimValue
                                             && c.Id != UserClaim.Id))
            {
                ModelState.AddModelError(string.Empty, "Claim này đã có");
                return Page();
            }
            UserClaim.ClaimType = Input.ClaimType;
            UserClaim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();

            StatusMessage = " Bạn vừa cập nhật claim";


            return RedirectToPage("./AddRole", new { id = user.Id });
        }
        public async Task<IActionResult> OnPostDeleteAsync(int? claimId)
        {
            if (claimId == null) return NotFound("Không tìm thấy user");

            UserClaim = _context.UserClaims.Where(c => c.Id == claimId).FirstOrDefault();

            user = await _userManager.FindByIdAsync(UserClaim.UserId);
            if (user == null)
            {
                return NotFound("Không tìm thấy user");
            }

            await _userManager.RemoveClaimAsync(user, new Claim(UserClaim.ClaimType, UserClaim.ClaimValue));

            StatusMessage = "Bạn vừa cập nhật claim";

            return RedirectToPage("./AddRole", new { id = user.Id });
        }
    }

}
