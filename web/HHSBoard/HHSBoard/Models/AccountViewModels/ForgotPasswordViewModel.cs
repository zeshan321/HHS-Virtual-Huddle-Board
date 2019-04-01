using System.ComponentModel.DataAnnotations;

namespace HHSBoard.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}