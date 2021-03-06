using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using InvestPlaceDB;
using Services.Services.LotService;
using Services.Services.ExtendedUserService;
using InvestPlace.Identity;
using Services.Services.FileService;
using Services.Services.CategoryService;
using Services.Services.BasketService;
using Services.Services.CashService;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.AspNetCore.Identity;
using Services.Services.QueryExchangeService;
using Services.Services.PuzzlePaintService;
using Services.Services.LotPresaveService;
using Services.Services.FaqService;
using Microsoft.AspNetCore.Identity.UI.Services;
using Services.Services.EmailSender;

namespace InvestPlace
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
#if DEBUG
            string connectionString = Configuration.GetConnectionString("LocalConnection");
#else
            string connectionString = Configuration.GetConnectionString("ProdConnection");
#endif

            services.AddDbContext<InvestPlaceContext>(options => options.UseSqlServer(connectionString));
            services.AddDefaultIdentity<ExtendedUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+1234567890�������������������������������������Ũ��������������������������";
                    options.ClaimsIdentity.RoleClaimType = "role";
                })
                .AddRoles<ExtendedRole>()
                .AddEntityFrameworkStores<InvestPlaceContext>()
                //.AddPasswordValidator<
                ;
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ExtendedUser>>();

            services.AddScoped<ILotService, LotService>();
            services.AddScoped<IExtendedUserService, ExtendedUserService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<ICashService, CashService>();
            services.AddScoped<IQueryExchangeService, QueryExchangeService>();
            services.AddScoped<IPuzzlePaintService, PuzzlePaintService>();
            services.AddScoped<ILotPresaveService, LotPresaveService>();
            services.AddScoped<IFaqService, FaqService>();

            services.AddTransient<IEmailSender, EmailSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
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
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
