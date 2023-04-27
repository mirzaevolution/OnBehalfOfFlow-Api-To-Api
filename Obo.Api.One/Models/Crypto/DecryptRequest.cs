using System.ComponentModel.DataAnnotations;

namespace Obo.Api.One.Models.Crypto
{
    public class DecryptRequest
    {
        [Required]
        public string Key { get; set; }
        [Required]
        public string CipherText { get; set; }
    }
}
