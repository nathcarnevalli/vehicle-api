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
    public ActionResult<Reserva> Post([FromBody] Reserva reserva)
    {
        var formato = "yyyy/MM/dd HH:mm:ss";

        if (reserva.DataInicio >= reserva.DataFim || (reserva.DataFim - reserva.DataInicio).TotalHours < 1 || reserva.DataInicio <= DateTime.Now)
        {
            return BadRequest("Datas inválidas, selecione um intervalo de pelo menos uma hora.");
        }

        var veiculoDisponivel = _uof.VeiculoRepository.GetVeiculosDisponiveis(reserva.DataInicio.ToString(formato), reserva.DataFim.ToString(formato)).FirstOrDefault(v => v.VeiculoId == reserva.VeiculoId);

        if (veiculoDisponivel is null)
        {
            return Conflict($"O veiculo de id = {reserva.VeiculoId} não se encontra disponível");
        }

        var reservaCriada = _uof.ReservaRepository.Create(reserva); /* Provisório - Confirmada - Cancelada */
        _uof.Commit();

        return new CreatedAtRouteResult("ObterReserva", new { id = reservaCriada.ReservaId }, reservaCriada);
    }

    [HttpPut("{id:int:min(1)}")]
    public ActionResult<Reserva> Put([FromBody] Reserva reserva, int id)
    {
        var formato = "yyyy/MM/dd HH:mm:ss";

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
        /* Alterada significa que o usuário deseja alterar o inicio portanto a referência deve ser a data de hoje */
        /* Data final alterada - significa que o usuário deseja adicionar mais dias a reserva */

        if (reserva.DataInicio >= reserva.DataFim || (reserva.DataFim - reserva.DataInicio).TotalHours < 1)
        {
            return BadRequest("Datas inválidas, selecione um intervalo de pelo menos uma hora");
        }

        var veiculoDisponivel = _uof.VeiculoRepository.GetVeiculosDisponiveis(reserva.DataInicio.ToString(formato), reserva.DataFim.ToString(formato)).FirstOrDefault(v => v.VeiculoId == reserva.VeiculoId);

        if (veiculoDisponivel is null)
        {
            return Conflict($"O veiculo de id = {reserva.VeiculoId} não se encontra disponível nesse intervalo");
        }

        var reservaCriada = _uof.ReservaRepository.Update(reserva); /* Provisório - Confirmada - Cancelada */
        _uof.Commit();

        return Ok(reservaCriada);
    }

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



