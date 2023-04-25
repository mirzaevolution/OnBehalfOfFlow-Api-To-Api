using System.ComponentModel.DataAnnotations;

namespace Obo.Api.One.Models.Hash
{
    public class SHA256Request
    {
        [Required]
        public string PlainText { get; set; }
    }
}
