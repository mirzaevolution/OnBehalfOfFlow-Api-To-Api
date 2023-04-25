using Microsoft.Identity.Web;

namespace Obo.Api.Two
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var configuration = builder.Configuration;
            services.AddMicrosoftIdentityWebApiAuthentication(configuration);
            services.AddDistributedMemoryCache();
            services.AddControllers();

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}