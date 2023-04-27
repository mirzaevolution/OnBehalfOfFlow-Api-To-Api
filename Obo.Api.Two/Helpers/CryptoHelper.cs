using System.Security.Cryptography;
using System.Text;

namespace Obo.Api.Two.Helpers
{
    public class CryptoHelper : ICryptoHelper
    {
        private readonly byte[] SALT = new byte[] { 138, 22, 216, 131, 224, 101, 9, 34, 82, 179, 210, 127, 155, 139, 184, 176, 139, 6, 179, 176, 176, 162, 87, 182, 249, 39, 40, 194, 11, 120, 95, 209 };
        private readonly byte[] IV = new byte[16] { 148, 139, 41, 92, 88, 228, 131, 64, 111, 206, 193, 176, 212, 212, 216, 152 };
        private Task<byte[]> GenerateKeyFromPassword(string password)
        {
            using Rfc2898DeriveBytes pbkf = new Rfc2898DeriveBytes(password, SALT, 10_000, HashAlgorithmName.SHA256);
            return Task.FromResult(pbkf.GetBytes(32));
        }

        public async Task<string> Encrypt(string plainText,
            string password)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] pwd = await GenerateKeyFromPassword(password);
            using (Aes aes = Aes.Create())
            {
                aes.Key = pwd;
                aes.IV = IV;
                using (ICryptoTransform cryptoTransform = aes.CreateEncryptor())
                {

                    byte[] cipherBytes = cryptoTransform.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(cipherBytes);
                }
            }
        }

        public async Task<string> Decrypt(string cipherText,
            string password)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] pwd = await GenerateKeyFromPassword(password);
            using (Aes aes = Aes.Create())
            {
                aes.Key = pwd;
                aes.IV = IV;
                using (ICryptoTransform cryptoTransform = aes.CreateDecryptor())
                {
                    byte[] plainBytes = cryptoTransform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    return Encoding.UTF8.GetString(plainBytes);
                }
            }
        }

    }
}
