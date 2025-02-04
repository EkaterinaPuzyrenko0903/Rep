using System.ComponentModel.DataAnnotations;

namespace Payments.Models
{
    public class ImportViewModel
    {
        [Required(ErrorMessage = "Please select a file.")]
        public IFormFile file { get; set; }
    }
}
