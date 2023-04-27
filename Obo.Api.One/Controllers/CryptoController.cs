using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Obo.Api.One.Models.Crypto;
using Obo.Api.One.Services;

namespace Obo.Api.One.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class CryptoController : ControllerBase
    {
        private readonly CryptoHttpService _cryptoHttpService;

        public CryptoController(CryptoHttpService cryptoHttpService)
        {
            _cryptoHttpService = cryptoHttpService;
        }

        [HttpPost(nameof(Encrypt))]
        [ProducesResponseType(typeof(EncryptResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Encrypt([FromBody] EncryptRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _cryptoHttpService.Encrypt(request);
                return Ok(result);

            }
            return BadRequest(ModelState);
        }


        [HttpPost(nameof(Decrypt))]
        [ProducesResponseType(typeof(DecryptResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Decrypt([FromBody] DecryptRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _cryptoHttpService.Decrypt(request);
                return Ok(result);
            }
            return BadRequest(ModelState);
        }
    }
}
