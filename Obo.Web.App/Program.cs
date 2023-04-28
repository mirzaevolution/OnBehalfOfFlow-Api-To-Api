using Microsoft.Identity.Web;

namespace Obo.Web.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var configuration = builder.Configuration;

            services.AddControllersWithViews();
            services.AddMicrosoftIdentityWebAppAuthentication(configuration)
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddDownstreamApi("OboApiOne", configuration.GetSection("OboApiOne"))
                .AddInMemoryTokenCaches();

            services.ConfigureApplicationCookie(options =>
            {
                options.ReturnUrlParameter = "redirectUrl";
                options.LoginPath = "/Auth/Login";
                options.LogoutPath = "/Auth/Logout";
            });

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Hash}/{action=Sha256}/{id?}");

            app.Run();
        }
    }
}