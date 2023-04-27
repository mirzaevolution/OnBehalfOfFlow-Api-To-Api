namespace Obo.Api.Two.Helpers
{
    public interface ICryptoHelper
    {

        Task<string> Encrypt(string plainText, string password);
        Task<string> Decrypt(string cipherText, string password);
    }
}
