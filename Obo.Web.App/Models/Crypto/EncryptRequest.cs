using System.ComponentModel.DataAnnotations;

namespace Obo.Api.Two.Models.Crypto
{
    public class EncryptRequest
    {
        [Required]
        public string Key { get; set; }
        [Required]
        public string PlainText { get; set; }
    }
}
