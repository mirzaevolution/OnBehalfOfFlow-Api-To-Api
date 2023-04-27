using IdentityModel.Client;
using Obo.Api.One.Helpers;
using Obo.Api.One.Models.Crypto;

namespace Obo.Api.One.Services
{
    public class CryptoHttpService
    {
        private readonly ITokenHelper _tokenHelper;
        private readonly HttpClient _httpClient;

        public CryptoHttpService(ITokenHelper tokenHelper, HttpClient httpClient)
        {
            _tokenHelper = tokenHelper;
            _httpClient = httpClient;
        }

        public async Task<EncryptResponse> Encrypt(EncryptRequest encryptRequest)
        {
            _httpClient.SetBearerToken(await _tokenHelper.GetAndRefreshToken());
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync<EncryptRequest>("/api/crypto/encrypt", encryptRequest);
            if (response.IsSuccessStatusCode)
            {
                EncryptResponse result = await response.Content.ReadFromJsonAsync<EncryptResponse>();
                return result;
            }
            throw new Exception($"{(int)response.StatusCode}");
        }

        public async Task<DecryptResponse> Decrypt(DecryptRequest decryptRequest)
        {
            _httpClient.SetBearerToken(await _tokenHelper.GetAndRefreshToken());
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync<DecryptRequest>("/api/crypto/decrypt", decryptRequest);
            if (response.IsSuccessStatusCode)
            {
                DecryptResponse result = await response.Content.ReadFromJsonAsync<DecryptResponse>();
                return result;
            }
            throw new Exception($"{(int)response.StatusCode}");
        }
    }
}
