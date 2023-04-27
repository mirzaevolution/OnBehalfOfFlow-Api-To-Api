using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Obo.Api.Two.Helpers;
using Obo.Api.Two.Models.Crypto;

namespace Obo.Api.Two.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class CryptoController : ControllerBase
    {
        private readonly ICryptoHelper _cryptoHelper;

        public CryptoController(ICryptoHelper cryptoHelper)
        {
            _cryptoHelper = cryptoHelper;
        }


        [HttpPost(nameof(Encrypt))]
        [ProducesResponseType(typeof(EncryptResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Encrypt([FromBody] EncryptRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _cryptoHelper.Encrypt(request.PlainText, request.Key);
                return Ok(new EncryptResponse
                {
                    ChiperText = result
                });
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
                var result = await _cryptoHelper.Decrypt(request.CipherText, request.Key);
                return Ok(new DecryptResponse
                {
                    PlainText = result
                });
            }
            return BadRequest(ModelState);
        }
    }
}
