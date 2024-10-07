using System.ComponentModel.DataAnnotations;

namespace ApiVeiculos.DTOs;

public class LoginModel
{
    [Required(ErrorMessage = "Username é obrigatório")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Password é obrigatória")]
    public string? Password { get; set; } 
}
