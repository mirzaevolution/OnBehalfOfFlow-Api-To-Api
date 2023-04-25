using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Obo.Web.App.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login(string redirectUrl = "/")
        {
            string returnUrl = Url.IsLocalUrl(redirectUrl) ? redirectUrl : Url.Action("Sha256", "Hash", null, Request.Scheme) ?? "/";
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = returnUrl
            }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [Authorize]
        public IActionResult Logout()
        {
            string returnUrl = Url.Action("Sha256", "Hash", null, Request.Scheme) ?? "/";
            return SignOut(new AuthenticationProperties
            {
                RedirectUri = returnUrl
            }, OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
