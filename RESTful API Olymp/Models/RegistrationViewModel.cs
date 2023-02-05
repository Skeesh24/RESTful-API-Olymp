using System.ComponentModel.DataAnnotations;

namespace RESTful_API_Olymp.Models
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Не введено имя")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Не введена фамилия")]
        public string? SecondName { get; set; }

        [Required(ErrorMessage="Не введен email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Пароль не был указан")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string? ConfirmPassword { get; set; }
    }
}
