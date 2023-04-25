using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Obo.Api.One.Helpers;
using Obo.Api.One.Models.Hash;

namespace Obo.Api.One.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class HashController : ControllerBase
    {
        private readonly HashHelper _hashHelper;

        public HashController(HashHelper hashHelper)
        {
            _hashHelper = hashHelper;
        }

        [HttpPost(nameof(SHA256))]
        [ProducesResponseType(typeof(SHA256Response), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SHA256([FromBody] SHA256Request request)
        {
            if (ModelState.IsValid)
            {
                string result = await _hashHelper.SHA256Hash(request.PlainText);
                return Ok(new SHA256Response
                {
                    Base64Hashed = result
                });
            }
            return BadRequest();
        }
    }
}
