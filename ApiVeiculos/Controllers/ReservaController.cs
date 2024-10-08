using ApiVeiculos.Models;
using ApiVeiculos.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ApiVeiculos.Controllers;

[ApiController]
[Route("[controller]")]
public class ReservasController : ControllerBase
{
    private readonly IUnitOfWork _uof;

    public ReservasController(IUnitOfWork uof)
    {
        _uof = uof;
    }

    [HttpGet]
    [Authorize(Policy = "GerenteOnly")]
    public ActionResult<IEnumerable<Reserva>> Get()
    {
        var reservas = _uof.ReservaRepository.GetAll();

        if (!reservas.Any())
        {
            return NoContent();
        }

        return Ok(reservas);
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterReserva")]
    [Authorize(Policy = "GerenteOnly")]
    public ActionResult<Reserva> Get(int id)
    {
        var reserva = _uof.ReservaRepository.Get(r => r.ReservaId == id);

        if (reserva is null)
        {
            return NotFound($"Reserva com id = {id} não encontrado");
        }

        return Ok(reserva);
    }

    [HttpPost]
    [Authorize(Policy = "AllRoles")]
    public ActionResult<Reserva> Post(Reserva reserva)
    {

        if (reserva.DataInicio >= reserva.DataFim ||  reserva.DataInicio <= DateTime.Now)
        {
            return BadRequest("Datas inválidas");
        }

        var veiculoDisponivel = _uof.VeiculoRepository.GetVeiculosDisponiveis(reserva.DataInicio, reserva.DataFim).FirstOrDefault(v => v.VeiculoId == reserva.VeiculoId);

        if (veiculoDisponivel is null)
        {
            return Conflict($"O veiculo de id = {reserva.VeiculoId} não se encontra disponível");
        }

        var reservaCriada = _uof.ReservaRepository.Create(reserva);
        _uof.Commit();

        return new CreatedAtRouteResult("ObterReserva", new { id = reservaCriada.ReservaId }, reservaCriada);
    }

    /* Alterar uma reserva */
    [HttpPut("{id:int:min(1)}")]
    [Authorize(Policy = "FuncionarioOrGerenteOnly")]
    public ActionResult<Reserva> Put([FromBody] Reserva reserva, int id)
    {
        var existeReserva = _uof.ReservaRepository.Get(r => r.ReservaId == id);

        if (existeReserva is null)
        {
            return NotFound($"Reserva de id = {id} não existe");
        }
        else if (id != existeReserva.ReservaId)
        {
            return BadRequest("Houve um erro...");
        }

        if (reserva.DataInicio >= reserva.DataFim)
        {
            return BadRequest("Datas inválidas");
        }

        /* Alteração de data */
        if (reserva.DataInicio != existeReserva.DataInicio || reserva.DataFim != existeReserva.DataFim)
        {
            if (reserva.DataInicio <= DateTime.Now)
            {
                return BadRequest("Datas inválidas");
            }

            var veiculoDisponivel = _uof.VeiculoRepository.GetVeiculosDisponiveis(reserva.DataInicio, reserva.DataFim).FirstOrDefault(v => v.VeiculoId == reserva.VeiculoId);

            if (veiculoDisponivel is null)
            {
                return Conflict($"O veiculo de id = {reserva.VeiculoId} não se encontra disponível nesse intervalo");
            }
        }
        else
        {
            var reservaEmAndamento = _uof.ReservaRepository.GetReservasVeiculo(id)?.FirstOrDefault(r =>
    r.Estado == Reserva.EstadoReserva.Confirmado || r.Estado == Reserva.EstadoReserva.Provisorio);

            if(reservaEmAndamento is not null)
            {
                return Conflict($"O veiculo de id = {reserva.VeiculoId} já possui uma reserva em andamento");
            }

        }

        var reservaAtualizada = _uof.ReservaRepository.Update(reserva); 
        _uof.Commit();

        return Ok(reservaAtualizada);
    }

    /* Cancelar uma reserva */
    [HttpDelete("{id:int:min(1)}")]
    [Authorize(Policy = "GerenteOnly")]
    public ActionResult<Reserva> Delete(int id)
    {
        var existeReserva = _uof.ReservaRepository.Get(r => r.ReservaId == id);

        if (existeReserva is null)
        {
            return NotFound($"Reserva de id = {id} não existe");
        }
        else if (existeReserva.ReservaId != id)
        {
            return BadRequest("Houve um erro...");
        }

        existeReserva.Estado = (Reserva.EstadoReserva) 2;

        var reservaCancelada = _uof.ReservaRepository.Delete(existeReserva);
        _uof.Commit();

        return Ok(reservaCancelada);
    }
}



