using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Obo.Api.Two.Models.Crypto;
using Obo.Web.App.Models.Crypto;
using System.Text.Json;

namespace Obo.Web.App.Controllers
{
    [Authorize]
    [AuthorizeForScopes(ScopeKeySection = "OboApiOne:Scopes")]
    public class CryptoController : Controller
    {
        private readonly IDownstreamApi _downstreamApi;

        private readonly ILogger<CryptoController> _logger;

        public CryptoController(IDownstreamApi downstreamApi, ILogger<CryptoController> logger)
        {
            _downstreamApi = downstreamApi;
            _logger = logger;
        }

        public IActionResult Encrypt()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Encrypt(EncryptViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = "/api/crypto/encrypt";
                _logger.LogInformation($"Calling {path}");
                _logger.LogInformation($"Param: {JsonSerializer.Serialize(model)}");
                EncryptResponse? response = null;
                try
                {
                    response = await _downstreamApi
                        .PostForUserAsync<EncryptRequest, EncryptResponse>("OboApiOne", new EncryptRequest
                        {
                            PlainText = model.PlainText,
                            Key = model.Password
                        }, options =>
                        {
                            options.RelativePath = path;
                        });
                    _logger.LogInformation($"Success result: {response.ChiperText}");
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occured");
                    _logger.LogError(ex.ToString());
                    model.Message = ex.Message;
                }
                model.CipherText = response?.ChiperText ?? string.Empty;
                model.Message = response != null ? string.Empty : "Response null from api server.";
                return View(model);
            }
            model.Message = "Invalid parameter!";
            return View(model);
        }


        public IActionResult Decrypt()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Decrypt(DecryptViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = "/api/crypto/decrypt";
                _logger.LogInformation($"Calling {path}");
                _logger.LogInformation($"Param: {JsonSerializer.Serialize(model)}");
                DecryptResponse? response = null;
                try
                {
                    response = await _downstreamApi
                        .PostForUserAsync<DecryptRequest, DecryptResponse>("OboApiOne", new DecryptRequest
                        {
                            CipherText = model.CipherText,
                            Key = model.Password
                        }, options =>
                        {
                            options.RelativePath = path;
                        });
                    _logger.LogInformation($"Success result: {response.PlainText}");
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occured");
                    _logger.LogError(ex.ToString());
                    model.Message = ex.Message;
                }
                model.PlainText = response?.PlainText ?? string.Empty;
                model.Message = response != null ? string.Empty : "Response null from api server.";
                return View(model);
            }
            model.Message = "Invalid parameter!";
            return View(model);
        }
    }
}
