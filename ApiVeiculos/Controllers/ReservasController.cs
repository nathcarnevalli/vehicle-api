using ApiVeiculos.DTOs;
using ApiVeiculos.Models;
using ApiVeiculos.Pagination;
using ApiVeiculos.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace ApiVeiculos.Controllers;

[ApiController]
[Route("[controller]")]
public class ReservasController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;

    public ReservasController(IUnitOfWork uof, IMapper mapper)
    {
        _uof = uof;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Policy = "GerenteOnly")]
    public async Task<ActionResult<IEnumerable<Reserva>>> Get([FromQuery] QueryStringParameters parameters)
    {
        var reservas = await _uof.ReservaRepository.GetReservasAsync(parameters);

        if (!reservas.Any())
        {
            return NoContent();
        }

        return Ok(new { Status = "200", Data = reservas });
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterReserva")]
    [Authorize(Policy = "GerenteOnly")]
    public async Task<ActionResult<Reserva>> Get(int id)
    {
        var reserva = await _uof.ReservaRepository.GetAsync(r => r.ReservaId == id);

        if (reserva is null)
        {
            return NotFound( new { Status = "404", Message = "Reserva não encontrado" });
        }

        return Ok(new { Status = "200", Data = reserva });
    }

    /* Criar uma reserva */
    [HttpPost]
    [Authorize(Policy = "AllRoles")]
    public async Task<ActionResult<Reserva>> Post(ReservaModel reserva)
    {
        if (reserva.DataInicio >= reserva.DataFim || reserva.DataInicio <= DateTime.Now)
        {
            return BadRequest(new { Status = "400", Message = "Datas inválidas" });
        }

        if (reserva.VeiculoId < 1)
        {
            return BadRequest(new { Status = "400", Message = "Veículo inválido" });
        }

        var veiculoDisponivel = await _uof.VeiculoRepository.GetVeiculoDisponivelByIdAsync(reserva.DataInicio, reserva.DataFim, reserva.VeiculoId);

        if (veiculoDisponivel is null)
        {
            return Conflict(new { Status = "409", Message = "O veículo não se encontra disponível nesse intervalo" });
        }

        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized(new { Status = "401", Message = "Usuário não autenticado" });
        }

        reserva.UserId = userIdClaim.Value;

        var reservaMap = _mapper.Map<Reserva>(reserva);

        var reservaCriada = _uof.ReservaRepository.Create(reservaMap);
        await _uof.CommitAsync();

        return new CreatedAtRouteResult("ObterReserva", new { Status = "201", Data = reservaCriada, Message = "Reserva criada com sucesso" });
    }

    /* Alterar uma reserva */
    [HttpPut("{id:int:min(1)}")]
    [Authorize(Policy = "FuncionarioOrGerenteOnly")]
    public async Task<ActionResult<Reserva>> Put(ReservaModel reserva, int id)
    {
        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (id != reserva.ReservaId || userIdClaim?.Value != reserva.UserId)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                   new { Status = "500", Message = "Houve um erro na alteração da reserva" });
        }

        var existeReserva = await _uof.ReservaRepository.GetAsync(r => r.ReservaId == id);

        if (existeReserva is null)
        {
            return NotFound( new { Status = "404", Message = "Reserva não encontrada"});
        }

        if(reserva.VeiculoId < 1)
        {
            return BadRequest( new { Status = "400", Message = "Veículo inválido"});
        }

        if (reserva.DataInicio >= reserva.DataFim)
        {
            return BadRequest(new { Status = "400", Message = "Datas inválidas" });
        }

        /* Alteração de data */
        if (reserva.DataInicio != existeReserva.DataInicio || reserva.DataFim != existeReserva.DataFim)
        {
            if (reserva.DataInicio <= DateTime.Now)
            {
                return BadRequest(new { Status = "400", Message = "Datas inválidas" });
            }

            var veiculoDisponivel = await _uof.VeiculoRepository.GetVeiculoDisponivelByIdAsync(reserva.DataInicio, reserva.DataFim, reserva.VeiculoId);

            if (veiculoDisponivel is null)
            {
                return Conflict(new {Status = "409", Message = "O veículo não se encontra disponível nesse intervalo" });
            }
        }

        var reservaMap = _mapper.Map<Reserva>(reserva);

        var reservaAtualizada = _uof.ReservaRepository.Update(reservaMap); 
        await _uof.CommitAsync();

        return Ok(new { Status = "200", Data = reservaAtualizada, Message = "Reserva atualizada com sucesso"});
    }

    /* Cancelar uma reserva */
    [HttpDelete("{id:int:min(1)}")]
    [Authorize(Policy = "GerenteOnly")]
    public async Task<ActionResult<Reserva>> Delete(int id)
    {
        var existeReserva = await _uof.ReservaRepository.GetAsync(r => r.ReservaId == id);

        if (existeReserva is null)
        {
            return NotFound(new { Status = "404", Message = "Reserva não encontrada" });
        }else if (existeReserva.Estado.Equals(Reserva.EstadoReserva.Cancelado))
        {
            return BadRequest(new { Status = "400", Message = "Reserva já foi deletada" });
        }

        existeReserva.Estado = Reserva.EstadoReserva.Cancelado;

        var reservaCancelada = _uof.ReservaRepository.Delete(existeReserva);
        await _uof.CommitAsync();

        return Ok(new { Status = "200", Data = reservaCancelada, Message = "Reserva deletada com sucesso" });
    }
}



