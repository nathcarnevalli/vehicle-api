using ApiVeiculos.Models;
using ApiVeiculos.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ApiVeiculos.Controllers;

[ApiController]
[Route("api/[controller]")]

public class VeiculosController : ControllerBase
{
    private readonly IUnitOfWork _uof;

    public VeiculosController(IUnitOfWork uof)
    {
        _uof = uof;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Veiculo>> Get()
    {
        var veiculos = _uof.VeiculoRepository.GetAll();

        if(veiculos is null)
        {
            return NotFound();
        }

        return Ok(veiculos);
    }
}

