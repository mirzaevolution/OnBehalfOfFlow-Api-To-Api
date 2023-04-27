using System.ComponentModel.DataAnnotations;

namespace Obo.Web.App.Models.Crypto
{
    public class EncryptViewModel
    {
        [Required]
        [Display(Name = "Plain Text")]
        public string PlainText { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string? CipherText { get; set; }
        public string? Message { get; set; }
    }
}
