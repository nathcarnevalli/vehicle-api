using ApiVeiculos.DTOs;
using ApiVeiculos.Models;
using ApiVeiculos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiVeiculos.Controllers;

[Route("[controller]")]
[ApiController]
public class AutenticacaoController : Controller
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AutenticacaoController(ITokenService tokenService,
                          UserManager<ApplicationUser> userManager,
                          RoleManager<IdentityRole> roleManager,
                          IConfiguration configuration,
                          ILogger<AutenticacaoController> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginModel login)
    {
        var user = await _userManager.FindByNameAsync(login.UserName!);

        if (user is not null && await _userManager.CheckPasswordAsync(user, login.Password!))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            /* Adiciona a Claim do tipo de Role */
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = _tokenService.GenerateAccessToken(authClaims,
                                                         _configuration);

            var refreshToken = _tokenService.GenerateRefreshToken();

            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"],
                               out int refreshTokenValidityInMinutes);

            user.RefreshTokenExpiryTime =
                            DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);

            user.RefreshToken = refreshToken;

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                Status = "200",
                Message = $"Olá {user.Name}",
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }
        return Unauthorized(new { Status = "401", Message = "Username ou senha incorretos" });
    }

    [HttpPost("Perfil")]
    [Authorize(Policy = "GerenteOnly")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var existeRole = await _roleManager.RoleExistsAsync(roleName);

        if (existeRole)
        {
            return BadRequest(new { Status = "400", Message = "Perfil já existe" });
        }

        var role = await _roleManager.CreateAsync(new IdentityRole(roleName));

        if (role.Succeeded)
        {
            return Ok(new { Status = "200", Message = $"Perfil {roleName} adicionado com sucesso!" });
        }else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Status = "500", Message = $"Houve um erro na criação do perfil {roleName}" });
        }
    }

    [HttpPost("Perfil/Adiciona/Usuario")]
    [Authorize(Policy = "GerenteOnly")]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if(user is not null)
        {
            var resultado = await _userManager.AddToRoleAsync(user, roleName);

            if (resultado.Succeeded) 
            {
                return Ok(new { Status = "200", Message = $"O usuário {user.Id} adicionado ao perfil {roleName}" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { Status = "500", Message = $"Houve um erro para adicionar o usuário {user.Id} ao perfil {roleName}" });
            }
        }

        return BadRequest(new { Status = "400", Message = "O usuário não foi encontrado" });
    }

}
