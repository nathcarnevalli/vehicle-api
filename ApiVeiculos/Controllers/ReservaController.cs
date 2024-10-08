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

        return Ok(new { Status = "200", Data = reservas });
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterReserva")]
    [Authorize(Policy = "GerenteOnly")]
    public ActionResult<Reserva> Get(int id)
    {
        var reserva = _uof.ReservaRepository.Get(r => r.ReservaId == id);

        if (reserva is null)
        {
            return NotFound( new { Status = "404", Message = "Reserva não encontrado" });
        }

        return Ok(new { Status = "200", Data = reserva });
    }

    [HttpPost]
    [Authorize(Policy = "AllRoles")]
    public ActionResult<Reserva> Post(Reserva reserva)
    {

        if (reserva.DataInicio >= reserva.DataFim ||  reserva.DataInicio <= DateTime.Now)
        {
            return BadRequest(new { Status = "400", Message = "Datas inválidas" });
        }

        var veiculoDisponivel = _uof.VeiculoRepository.GetVeiculosDisponiveis(reserva.DataInicio, reserva.DataFim).FirstOrDefault(v => v.VeiculoId == reserva.VeiculoId);

        if (veiculoDisponivel is null)
        {
            return Conflict(new { Status = "409", Message = "Veiculo não se encontra disponível nesse intervalo" });
        }

        var reservaCriada = _uof.ReservaRepository.Create(reserva);
        _uof.Commit();

        return new CreatedAtRouteResult("ObterReserva", new { Status = "201", Data = reservaCriada, Message = "Reserva criada com sucesso" });
    }

    /* Alterar uma reserva */
    [HttpPut("{id:int:min(1)}")]
    [Authorize(Policy = "FuncionarioOrGerenteOnly")]
    public ActionResult<Reserva> Put([FromBody] Reserva reserva, int id)
    {
        var existeReserva = _uof.ReservaRepository.Get(r => r.ReservaId == id);

        if (existeReserva is null)
        {
            return NotFound( new { Status = "404", Message = "Reserva não encontrada"});
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

            var veiculoDisponivel = _uof.VeiculoRepository.GetVeiculosDisponiveis(reserva.DataInicio, reserva.DataFim).FirstOrDefault(v => v.VeiculoId == reserva.VeiculoId);

            if (veiculoDisponivel is null)
            {
                return Conflict(new {Status = "409", Message = "Veículo não se encontra disponível nesse intervalo" });
            }
        }
        else
        {
            var reservaEmAndamento = _uof.ReservaRepository.GetReservasVeiculo(id)?.FirstOrDefault(r =>
    r.Estado == Reserva.EstadoReserva.Confirmado || r.Estado == Reserva.EstadoReserva.Provisorio);

            if(reservaEmAndamento is not null)
            {
                return Conflict(new {Status = "409", Message = "Veículo já possui uma reserva em andamento" });
            }

        }

        var reservaAtualizada = _uof.ReservaRepository.Update(reserva); 
        _uof.Commit();

        return Ok(new { Status = "200", Data = reservaAtualizada, Message = "Reserva atualizada com sucesso"});
    }

    /* Cancelar uma reserva */
    [HttpDelete("{id:int:min(1)}")]
    [Authorize(Policy = "GerenteOnly")]
    public ActionResult<Reserva> Delete(int id)
    {
        var existeReserva = _uof.ReservaRepository.Get(r => r.ReservaId == id);

        if (existeReserva is null)
        {
            return NotFound(new { Status = "404", Message = "Reserva não encontrada" });
        }else if (existeReserva.Estado.Equals(Reserva.EstadoReserva.Cancelado))
        {
            return BadRequest(new { Status = "400", Message = "Reserva já foi deletada" });
        }

        existeReserva.Estado = (Reserva.EstadoReserva) 2;

        var reservaCancelada = _uof.ReservaRepository.Delete(existeReserva);
        _uof.Commit();

        return Ok(new { Status = "200", Data = reservaCancelada, Message = "Reserva deletada com sucesso" });
    }
}



