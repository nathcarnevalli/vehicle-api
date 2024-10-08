using ApiVeiculos.DTOs;
using ApiVeiculos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiVeiculos.Controllers;

[Route("[controller]")]
[ApiController]
public class UsuariosController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    public UsuariosController(UserManager<ApplicationUser> userManager) 
    { 
        _userManager = userManager;
    }

    [HttpGet]
    //[Authorize(Policy = "FuncionarioOrGerenteOnly")]
    public async Task<IActionResult> GetAll()
    {
        var usuarios = await _userManager.Users.ToListAsync();

        if (!usuarios.Any())
        {
            return NoContent();
        }

        return Ok(new { Status = "200", Data = usuarios });
    }

    [HttpGet("{id:alpha}", Name = "ObterUsuario")]
    //[Authorize(Policy = "FuncionarioOrGerenteOnly")]
    public async Task<IActionResult> Get(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);

        if (usuario is null)
        {
            return NotFound(new { Status = "404", Message = "Usuário não encontrado"});
        }

        return Ok(new { Status = "200", Data = usuario });
    }

    /* Atualizar um usuário */
    [HttpPost]
    public async Task<IActionResult> Post(RegisterModel register)
    {
        var existeUsuario = await _userManager.FindByNameAsync(register.Username!);

        if (existeUsuario?.CPF == register.CPF || register.Email == existeUsuario?.Email)
        {
            return BadRequest(new { Status = "400", Message = "O usuário já está cadastrado" });
        }

        ApplicationUser user = new()
        {
            Email = register.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = register.Username,
            CPF = register.CPF,
            Name = register.Nome
        };

        await _userManager.CreateAsync(user, register.Password!);
        var resultado = await _userManager.AddToRoleAsync(user, "Cliente");

        if (!resultado.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                   new { Status = "500", Message = "Houve um erro na criação de usuário" });
        }

        return new CreatedAtRouteResult("ObterUsuario", new { Status = "200", Message = "Usuário criado com sucesso!" });

    }

    /* Alterar um usuário */
    [HttpPut("{id:alpha}")]
    //[Authorize(Policy = "GerenteOnly")]
    public async Task<IActionResult> Put(string id, UserModel usuario)
    {
        var existeUsuario = await _userManager.FindByIdAsync(id);

        if (existeUsuario is null)
        {
            return NotFound(new { Status = "404", Message = "Usuário não encontrado" });
        }

        existeUsuario.Name = usuario.Name;
        await _userManager.SetUserNameAsync(existeUsuario, usuario.Username);
        await _userManager.ChangeEmailAsync(existeUsuario, existeUsuario.Email!, usuario.Email!);
        await _userManager.ChangePasswordAsync(existeUsuario, existeUsuario.PasswordHash!, usuario.Password!);


        var usuarioAtualizado = await _userManager.UpdateAsync(existeUsuario);

        return Ok(new { Status = "200", Data = usuarioAtualizado, Message = "Informações atualizadas com sucesso" });
    }

    /* Deletar um usuário */
    [HttpDelete("{id:alpha}")]
    //[Authorize(Policy = "GerenteOnly")]
    public async Task<IActionResult> Delete(string id)
    {
        var existeUsuario = await _userManager.FindByIdAsync(id);

        if (existeUsuario is null)
        {
            return NotFound(new { Status = "404", Message = "Usuário não encontrado" });
        } else if (existeUsuario.Estado.Equals(ApplicationUser.EstadoUsuario.Inativo))
        {
            return BadRequest(new { Status = "400", Message = "Usuário já foi deletado"});
        }

        existeUsuario.Estado = ApplicationUser.EstadoUsuario.Inativo;

        var usuarioAtualizado = await _userManager.UpdateAsync(existeUsuario);

        return Ok(new { Status = "200", Data = usuarioAtualizado, Message = "Usuário deletado com sucesso" });
    }
}
