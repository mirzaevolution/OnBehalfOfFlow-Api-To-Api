using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Obo.Api.One.Helpers;
using Obo.Api.One.Services;
using System.IdentityModel.Tokens.Jwt;

namespace Obo.Api.One
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var configuration = builder.Configuration;

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddMicrosoftIdentityWebApiAuthentication(configuration);
            services.AddDistributedMemoryCache();
            services.AddScoped<IHashHelper, HashHelper>();
            services.AddScoped<ITokenHelper, TokenHelper>();
            services.AddControllers();
            services.AddHttpClient<CryptoHttpService>(options =>
            {
                options.BaseAddress = new Uri(configuration["OboApiTwo:BaseUrl"]);
            });
            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, config =>
            {
                config.MapInboundClaims = true;
            });
            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}