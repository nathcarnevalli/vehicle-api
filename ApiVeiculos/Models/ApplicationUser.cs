using Microsoft.AspNetCore.Identity;

namespace ApiVeiculos.Models;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string? Name { get; set; }

    public string? CPF {  get; set; }

    public EstadoUsuario Estado { get; set; }

    public enum EstadoUsuario
    {
        Ativo,
        Inativo
    }
}