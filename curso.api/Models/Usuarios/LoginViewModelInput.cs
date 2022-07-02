using System.ComponentModel.DataAnnotations;

namespace curso.api.Models.Usuarios
{
    public class LoginViewModelInput
    {
        [Required(ErrorMessage = "O Login é obrigatório!")]
        public string Login { get; set; }
        [Required(ErrorMessage = "A Senha é obrigatória!")]
        public string Senha { get; set; }

    }
}
