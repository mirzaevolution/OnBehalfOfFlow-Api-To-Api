using Microsoft.Identity.Web;
using Obo.Api.Two.Helpers;

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
            services.AddScoped<ICryptoHelper, CryptoHelper>();
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