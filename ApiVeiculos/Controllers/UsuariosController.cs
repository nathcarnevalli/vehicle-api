using ApiVeiculos.DTOs;
using ApiVeiculos.Models;
using ApiVeiculos.Pagination;
using ApiVeiculos.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace ApiVeiculos.Controllers;

[Route("[controller]")]
[ApiController]
public class UsuariosController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    public UsuariosController(UserManager<ApplicationUser> userManager, IMapper mapper, IUserService userService) 
    { 
        _userManager = userManager;
        _mapper = mapper;
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Policy = "FuncionarioOrGerenteOnly")]
    public async Task<IActionResult> GetAll([FromQuery]QueryStringParameters parameters)
    {
        var usuariosNoMap = await _userManager.Users.ToListAsync();

        if (!usuariosNoMap.Any())
        {
            return NoContent();
        }

        var usuariosMap = _mapper.Map<IEnumerable<UserModel>>(usuariosNoMap);

        var usuariosOrdenados = usuariosMap.OrderBy(u => u.Id).AsQueryable();

        var usuarios = usuariosOrdenados.ToPagedList(parameters.PageNumber, parameters.PageSize);

        return Ok(new { Status = "200", Data = usuarios });
    }

    [HttpGet("{id}", Name = "ObterUsuario")]
    [Authorize(Policy = "FuncionarioOrGerenteOnly")]
    public async Task<IActionResult> Get(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);

        if (usuario is null)
        {
            return NotFound(new { Status = "404", Message = "Usuário não encontrado"});
        }

        var usuariosMap = _mapper.Map<UserModel>(usuario);

        return Ok(new { Status = "200", Data = usuariosMap });
    }

    /* Criar um usuário */
    [HttpPost]
    public async Task<IActionResult> Post(RegisterModel register)
    {

        if (!_userService.VerificaCpf(register.CPF!))
        {
            return BadRequest(new { Status = "400", Message = "CPF inválido" });
        }

        if (!_userService.VerificaEmail(register.Email!))
        {
            return BadRequest(new { Status = "400", Message = "Email inválido" });
        }

        if (!await _userService.VerificaFormsAsync(register.Email!, register.CPF!, register.Username!))
        {
            return BadRequest(new { Status = "400", Message = "Usuário já cadastrado" });
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

        var usuariosMap = _mapper.Map<UserModel>(user);

        return new CreatedAtRouteResult("ObterUsuario", new { Status = "200", Data = usuariosMap, Message = "Usuário criado com sucesso!" });

    }

    /* Alterar um usuário */
    [HttpPut("{id}")]
    [Authorize(Policy = "GerenteOnly")]
    public async Task<IActionResult> Put(string id, UserModel usuario)
    {
        var existeUsuario = await _userManager.FindByIdAsync(id);

        if (id != usuario.Id)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                   new { Status = "500", Message = "Houve um erro na alteração de usuário" });
        }

        if (existeUsuario is null)
        {
            return NotFound(new { Status = "404", Message = "Usuário não encontrado" });
        }

        if (!_userService.VerificaEmail(usuario.Email!))
        {
            return BadRequest(new { Status = "400", Message = "Email inválido" });
        }

        var (usuarioAtualizado, mensagem) = await _userService.AlteraUsuarioAsync(usuario, existeUsuario);

        if (usuarioAtualizado is null)
        {
            return BadRequest(new { Status = "400", Message = mensagem });
        }

        await _userManager.UpdateAsync(usuarioAtualizado);

        return Ok(new { Status = "200", Data = usuario, Message = "Informações atualizadas com sucesso" });
    }

    /* Deletar um usuário */
    [HttpDelete("{id}")]
    [Authorize(Policy = "GerenteOnly")]
    public async Task<IActionResult> Delete(string id)
    {
        var existeUsuario = await _userManager.FindByIdAsync(id);

        if (existeUsuario is null)
        {
            return NotFound(new { Status = "404", Message = "Usuário não encontrado" });
        } 
        else if (existeUsuario.Estado.Equals(ApplicationUser.EstadoUsuario.Inativo))
        {
            return BadRequest(new { Status = "400", Message = "Usuário já foi deletado"});
        }

        existeUsuario.Estado = ApplicationUser.EstadoUsuario.Inativo;

        await _userManager.UpdateAsync(existeUsuario);

        var usuariosMap = _mapper.Map<UserModel>(existeUsuario);

        return Ok(new { Status = "200", Data = usuariosMap, Message = "Usuário deletado com sucesso" });
    }
}
