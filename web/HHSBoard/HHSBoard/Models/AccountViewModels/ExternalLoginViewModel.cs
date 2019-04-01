using System.ComponentModel.DataAnnotations;

namespace HHSBoard.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}