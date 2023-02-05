using System.ComponentModel.DataAnnotations;

namespace RESTful_API_Olymp.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Email не был указан")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Пароль не был указан")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string? ConfirmPassword { get; set; }
    }
}
