using System.ComponentModel.DataAnnotations;

namespace Obo.Web.App.Models.Hash
{
    public class HashViewModel
    {
        [Required]
        [Display(Name = "Plain Text")]
        public string Input { get; set; }
        public string? Output { get; set; }
        public string? Message { get; set; }
    }
}
