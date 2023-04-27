using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Identity.Client;
using Obo.Api.One.Models;
using System.Security.Claims;
using System.Text.Json;

namespace Obo.Api.One.Helpers
{
    public class TokenHelper : ITokenHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IDistributedCache _distributedCache;
        private readonly IConfiguration _configuration;
        public TokenHelper(IHttpContextAccessor contextAccessor,
            IDistributedCache distributedCache,
            IConfiguration configuration)
        {
            _contextAccessor = contextAccessor;
            _distributedCache = distributedCache;
            _configuration = configuration;
        }

        public async Task<string> GetAndRefreshToken()
        {
            var httpContext = _contextAccessor.HttpContext;

            string userName = httpContext.User.FindFirstValue("email");

            UserAssertionTokenModel assertionTokenModel = await GetTokenByUsername(userName);
            if (assertionTokenModel != null &&
                assertionTokenModel.UserName.Equals(userName) &&
                assertionTokenModel.ExpiredDateUtc > DateTimeOffset.UtcNow.AddMinutes(-1))
            {
                return assertionTokenModel.AccessToken;
            }
            else
            {
                string token = await httpContext.GetTokenAsync(OidcConstants.TokenTypes.AccessToken);

                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("Access Token is null or empty");
                }

                //OidcConstants.GrantTypes.JwtBearer = urn:ietf:params:oauth:grant-type:jwt-bearer
                UserAssertion userAssertion = new UserAssertion(token, OidcConstants.GrantTypes.JwtBearer);

                string[] scopes = _configuration.GetSection("OboApiTwo:Scopes").Get<string[]>() ?? new string[0];

                IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
                    .Create(_configuration["AzureAd:ClientId"])
                        .WithAuthority($"{_configuration["AzureAd:Instance"]}{_configuration["AzureAd:TenantId"]}")
                        .WithClientSecret(_configuration["AzureAd:ClientSecret"])
                        .Build();

                AuthenticationResult result = await app.AcquireTokenOnBehalfOf(scopes, userAssertion)
                    .ExecuteAsync();

                await _distributedCache.SetStringAsync(userName,
                    JsonSerializer.Serialize(new UserAssertionTokenModel
                    {
                        AccessToken = result.AccessToken,
                        ExpiredDateUtc = result.ExpiresOn,
                        UserName = userName
                    }));

                return result.AccessToken;

            }
        }

        private async Task<UserAssertionTokenModel> GetTokenByUsername(string username)
        {
            string json = await _distributedCache.GetStringAsync(username);
            return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<UserAssertionTokenModel>(json);
        }



    }
}
