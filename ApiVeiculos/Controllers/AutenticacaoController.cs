using APICatalogo.Models;
using ApiVeiculos.DTOs;
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
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }
        return Unauthorized();
    }

    [HttpPost("Cadastro")]
    public async Task<IActionResult> Register([FromBody] RegisterModel register)
    {
        var existeUsuario = await _userManager.FindByNameAsync(register.Username!);

        if (existeUsuario?.CPF == register.CPF)
        {
            return BadRequest("O usuário já está cadastrado");
        }

        ApplicationUser user = new()
        {
            Email = register.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = register.Username,
            CPF = register.CPF,
            Name = register.Nome
        };

        var result = await _userManager.CreateAsync(user, register.Password!);

        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                   new { Status = "Error", Message = "Houve you erro na criação de usuário" });
        }

        return Ok(new { Status = "Success", Message = "Usuário criado com sucesso!" });

    }

    [HttpPost("Perfil")]
    [Authorize]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var existeRole = await _roleManager.RoleExistsAsync(roleName);

        if (existeRole)
        {
            return BadRequest("Esse perfil já existe");
        }

        var role = await _roleManager.CreateAsync(new IdentityRole(roleName));

        if (role.Succeeded)
        {
            return Ok("Perfil adicionado com sucesso!");
        }else
        {
            return BadRequest("Houve um erro na criação do perfil");
        }
    }

    [HttpPost("Perfil/Adiciona/Usuario")]
    [Authorize]

    /*Adicionar as claims na rota*/
    public async Task<IActionResult> AddRole(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if(user is not null)
        {
            var resultado = await _userManager.AddToRoleAsync(user, roleName);

            if (resultado.Succeeded) {
                return Ok(new { Status = "200", Message = $"O usuário {user.Email} adicionado ao perfil {roleName}" });
            }
            else
            {
                return Ok(new { Status = "200", Message = $"O usuário {user.Email} adicionado ao perfil {roleName}" });
            }
        }

        return BadRequest("O usuário não foi encontrado");
    }

}
