using System.ComponentModel.DataAnnotations;

namespace Company.Demo.PL.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "يرجى إدخال بريد إلكتروني صحيح")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; } = string.Empty;
    }
}
