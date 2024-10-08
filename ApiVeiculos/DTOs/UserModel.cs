using System.ComponentModel.DataAnnotations;

namespace ApiVeiculos.DTOs;

public class UserModel
{
    [Required(ErrorMessage = "Username é obrigatório")]
    public string? Username { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "Email é obrigatório")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password é obrigatória")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "CPF é obrigatório")]
    public string? CPF { get; set; }
}
