using Microsoft.AspNetCore.Identity;

namespace APICatalogo.Models;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string? Name { get; set; }
    public string? CPF {  get; set; }
}