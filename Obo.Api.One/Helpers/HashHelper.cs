using System.Security.Cryptography;
using System.Text;

namespace Obo.Api.One.Helpers
{
    public class HashHelper
    {
        public Task<string> SHA256Hash(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            using SHA256 hash = SHA256.Create();
            byte[] hashedBytes = hash.ComputeHash(inputBytes);
            string hashedBase64 = Convert.ToBase64String(hashedBytes);
            return Task.FromResult(hashedBase64);
        }
    }
}
