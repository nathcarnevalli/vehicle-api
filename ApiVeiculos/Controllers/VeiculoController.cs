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
            return BadRequest("Houve um erro..."); 
        }

        return Ok(veiculos); 
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterVeiculo")]
    public ActionResult<Veiculo> Get(int id)
    {
        var veiculo = _uof.VeiculoRepository.Get(v => v.VeiculoId == id);

        if(veiculo is null)
        {
            return NotFound($"Veículo com id = {id} não encontrado");
        }

        return Ok(veiculo); 
    }

    [HttpPost]
    public ActionResult<Veiculo> Post([FromBody] Veiculo veiculo)
    {
        var existeVeiculo = _uof.VeiculoRepository.Get(v => v.Placa == veiculo.Placa);

        if(existeVeiculo is not null)
        {
            return Conflict($"Veículo de placa {veiculo.Placa} já existe"); 
        }

        var veiculoCriado = _uof.VeiculoRepository.Create(veiculo);
        _uof.Commit();

        return new CreatedAtRouteResult("ObterVeiculo", new {id = veiculoCriado.VeiculoId}, veiculoCriado); 
    }

    [HttpPut("{id:int:min(1)}")]
    public ActionResult<Veiculo> Put([FromBody] Veiculo veiculo, int id) 
    {
        var existeVeiculo = _uof.VeiculoRepository.Get(v => v.VeiculoId == id);

        if(existeVeiculo is null)
        {
            return NotFound($"Veículo de id = {id} não existe");
        }
        else if(veiculo.VeiculoId != id)
        {
            return BadRequest("Houve um erro...");
        }

        var veiculoAtualizado = _uof.VeiculoRepository.Update(veiculo);
        _uof.Commit();

        return Ok(veiculo);
    }

    [HttpDelete("{id:int:min(1)}")] /*Persistência de dados*/
    public ActionResult<Veiculo> Delete(int id)
    {
        var existeVeiculo = _uof.VeiculoRepository.Get(v => v.VeiculoId == id);

        if(existeVeiculo is null)
        {
            return NotFound($"Veículo de id = {id} não existe");
        }
        else if (existeVeiculo.VeiculoId != id)
        {
            return BadRequest("Houve um erro...");
        }

        existeVeiculo.Estado = (Veiculo.EstadoVeiculo) 2;

        var veiculoDeletado = _uof.VeiculoRepository.Delete(existeVeiculo, id);
        _uof.Commit();

        return Ok(veiculoDeletado);
    }
}

