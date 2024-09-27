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

    [HttpGet("{id:int:min(1)}")]
    public ActionResult<Reserva> Get(int id)
    {
        var reserva = _uof.ReservaRepository.Get(r => r.ReservaId == id);

        if (reserva is null)
        {
            return NotFound($"Reserva com id = {id} não encontrado");
        }

        return Ok(reserva);
    }
}



