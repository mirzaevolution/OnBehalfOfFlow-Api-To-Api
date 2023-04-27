using System.ComponentModel.DataAnnotations;

namespace Obo.Web.App.Models.Crypto
{
    public class DecryptViewModel
    {
        [Required]
        [Display(Name = "Cipher Text")]
        public string CipherText { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string? PlainText { get; set; }
        public string? Message { get; set; }
    }
}
