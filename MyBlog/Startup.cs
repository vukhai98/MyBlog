using MyBlog.Models;
using MyBlog.Security.Requirements;
using MyBlog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
             services.AddHttpContextAccessor();
            services.AddOptions();
            var mailsetting = Configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailsetting);
            services.AddSingleton<IEmailSender, SendMailService>();

            services.AddRazorPages().AddRazorRuntimeCompilation();
                //.AddMvcOptions(o=>o.Filters.Add(new AuthorizeFilter())).AddRazorRuntimeCompilation(); //Globel Filter
            services.AddDbContext<MyBlogContext>(options => {
                string connectString = Configuration.GetConnectionString("MyBlogContext");
                options.UseSqlServer(connectString);
            });

            //// Đăng kí Identity
            //services.AddIdentity<AppUser, IdentityRole>()
            //        .AddEntityFrameworkStores<MyBlogContext>()
            //        .AddDefaultTokenProviders();
            // Đăng kí Identity
            services.AddDefaultIdentity<AppUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<MyBlogContext>()
                    .AddDefaultTokenProviders();
           
            // Truy cập IdentityOptions
            services.Configure<IdentityOptions>(options => {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;
                // Xác thực số điện thoại

                //
                options.SignIn.RequireConfirmedAccount = true;
                
            });

            services.Configure<RouteOptions>(options => {
                options.LowercaseUrls = true; // url chữ thường
                options.LowercaseQueryStrings = false; // không bắt query trong url phải in thường
            });

            services.ConfigureApplicationCookie(options => {
                // options.Cookie.HttpOnly = true;  
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = $"/login/";
                options.LogoutPath = $"/logout/";
                options.AccessDeniedPath = $"/khongduoctruycap.html";
            });
            services.AddAuthentication()
                    .AddGoogle(options => {
                      var gConfig =  Configuration.GetSection("Authentication:Google");
                        options.ClientId = gConfig["ClientId"];
                        options.ClientSecret = gConfig["ClientSecret"];
                        options.CallbackPath = "/dangnhaptugoogle";
                    })
                    .AddFacebook(options => {
                        var fConfig = Configuration.GetSection("Authentication:Facebook");
                        options.ClientId = fConfig["AppId"];
                        options.ClientSecret = fConfig["AppSecret"];
                        options.CallbackPath = "/dang-nhap-tu-facebook";
                    });

            services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

            services.AddAuthorization(options =>
            {

                options.AddPolicy("AllowEditRole", policyBuilder =>
                {
                    // Dieu kien cua Policy
                    policyBuilder.RequireAuthenticatedUser();
                    //policyBuilder.RequireRole("Admin");
                    //policyBuilder.RequireRole("Vip");
                    policyBuilder.RequireClaim("canedit", "user");
                });

                options.AddPolicy("IsGenZ", policyBuilder =>
                {
                    // Dieu kien cua Policy
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.Requirements.Add(new GenZRequirement());
                });

                options.AddPolicy("ShowAdminMenu", policyBuilder =>
                {
                    policyBuilder.RequireRole("Admin");
                });
                options.AddPolicy("CanUpdateArticle", policyBuilder =>
                {
                    
                    policyBuilder.Requirements.Add(new ArticleUpdateRequirement());
                });

            });

            services.AddTransient<IAuthorizationHandler, AppAuthorizationHandler>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                //endpoints.MapRazorPages().RequireAuthorization();
            });
          
        }
    }
}
/*
    CREATE , READ, UPDATE, DELETE (CRUT) 

    Identity:
       Athentication : Xác định danh tính -> Login , logot
       Authorization: Xác thực quyền truy cập 
       Role-based athorization- xác thực quyền theo vai trò
        - Role (vai trò): (Adim,Editor,Manager,Member...)

        /Areas/Admin/Pages/Role
            Index
            Create
            Edit
            Delete
       * Policy-based authorization
       * Claim-based authorization






       Quản lý user: Sing up , User , Role...................
        
    /Identity/Account/Login
    /Identity/Account/Manage
//https://localhost:5001/dangnhaptugoogle

 */
