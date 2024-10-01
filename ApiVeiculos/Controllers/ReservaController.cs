using ApiVeiculos.Models;
using ApiVeiculos.Repositories;
using Microsoft.AspNetCore.Mvc;
namespace ApiVeiculos.Controllers;

[ApiController]
[Route("Api/[controller]")]
public class ReservasController : ControllerBase
{
    private readonly IUnitOfWork _uof;

    public ReservasController(IUnitOfWork uof)
    {
        _uof = uof;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Reserva>> Get()
    {
        var reservas = _uof.ReservaRepository.GetAll();

        if (reservas is null)
        {
            return NoContent();
        }

        return Ok(reservas);
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterReserva")]
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
    public ActionResult<Reserva> Put(Reserva reserva, int id)
    {
        var reservaExiste = _uof.ReservaRepository.Get(r => r.ReservaId == id);

        if(reservaExiste is null)
        {
            return NotFound($"Reserva de id = {id} não existe");
        }
        else if (id != reserva.ReservaId)
        {
            return BadRequest("Houve um erro...");
        }

        /* Lógica da data de inicio - alterada ou não */
        /* Alterada significa que o usuário deseja alterar a data de inicio e a final */
        /* Data final alterada - significa que o usuário deseja adicionar mais dias a reserva */

        if (reserva.DataInicio >= reserva.DataFim)
        {
            return BadRequest("Datas inválidas");
        }

        if(reserva.DataInicio != reservaExiste.DataInicio)
        {
            if(reserva.DataInicio <= DateTime.Now)
            {
                return BadRequest("Datas inválidas");
            }
        }

        var veiculoDisponivel = _uof.VeiculoRepository.GetVeiculosDisponiveis(reserva.DataInicio, reserva.DataFim).FirstOrDefault(v => v.VeiculoId == reserva.VeiculoId);

        if (veiculoDisponivel is null)
        {
            return Conflict($"O veiculo de id = {reserva.VeiculoId} não se encontra disponível nesse intervalo");
        }

        var reservaAtualizada = _uof.ReservaRepository.Update(reserva); 
        _uof.Commit();

        return Ok(reservaAtualizada);
    }

    /* Cancelar uma reserva */
    [HttpDelete("{id:int:min(1)}")]
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



