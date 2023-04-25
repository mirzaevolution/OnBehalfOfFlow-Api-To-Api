using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Obo.Web.App.Models.Hash;

namespace Obo.Web.App.Controllers
{
    [Authorize]
    [AuthorizeForScopes(ScopeKeySection = "OboApiOne:Scopes")]
    public class HashController : Controller
    {
        private readonly IDownstreamApi _downstreamApi;

        private readonly ILogger<HashController> _logger;

        public HashController(IDownstreamApi downstreamApi, ILogger<HashController> logger)
        {
            _downstreamApi = downstreamApi;
            _logger = logger;
        }

        public IActionResult Sha256()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Sha256(HashViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = "/api/hash/sha256";
                _logger.LogInformation($"Calling {path}");
                _logger.LogInformation($"Param: {model.Input}");
                HashResponse? response = null;
                try
                {
                    response = await _downstreamApi.PostForUserAsync<HashRequest, HashResponse>("OboApiOne", new HashRequest
                    {
                        PlainText = model.Input
                    }, options =>
                    {
                        options.RelativePath = path;
                    });
                    _logger.LogInformation($"Success result: {response.Base64Hashed}");
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occured");
                    _logger.LogError(ex.ToString());
                    model.Message = ex.Message;
                }
                model.Output = response?.Base64Hashed ?? string.Empty;
                model.Message = response != null ? string.Empty : "Response null from api server.";
                return View(model);
            }
            model.Message = "Invalid parameter!";
            return View(model);
        }
    }
}
