using Obo.Api.One.Models.Crypto;

namespace Obo.Api.One.Services
{
    public interface ICryptoHttpService
    {
        Task<EncryptResponse> Encrypt(EncryptRequest encryptRequest);
        Task<DecryptResponse> Decrypt(DecryptRequest decryptRequest);
    }
}
