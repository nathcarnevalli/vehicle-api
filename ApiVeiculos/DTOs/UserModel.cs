using System.ComponentModel.DataAnnotations;

namespace ApiVeiculos.DTOs;

public class UserModel
{
    public string? Id { get; set; }
    [Required(ErrorMessage = "Username é obrigatório")]
    public string? UserName { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "Email é obrigatório")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password é obrigatória")]
    public string? PasswordHash { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "CPF é obrigatório")]
    public string? CPF { get; set; }

    public EstadoUsuario Estado { get; set; }
    public enum EstadoUsuario
    {
        Ativo,
        Inativo
    }
}
