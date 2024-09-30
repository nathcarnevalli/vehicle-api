﻿using ApiVeiculos.Models;
using ApiVeiculos.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace ApiVeiculos.Controllers;

[ApiController]
[Route("Api/[controller]")]

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
            return NoContent();
        }

        return Ok(veiculos); 
    }

    [HttpGet("Disponiveis")]
    public ActionResult<IEnumerable<Veiculo>> GetVeiculosDisponiveis([BindRequired][FromQuery] string dataInicio, [BindRequired][FromQuery] string dataFim)
    {
        if (DateTime.Parse(dataInicio) >= DateTime.Parse(dataFim) || DateTime.Parse(dataInicio) <= DateTime.Now || (DateTime.Parse(dataFim) - DateTime.Parse(dataInicio)).TotalHours < 1)
        {
            return BadRequest("Datas inválidas, selecione um intervalo de pelo menos uma hora");
        }

        var veiculos = _uof.VeiculoRepository.GetVeiculosDisponiveis(dataInicio, dataFim).AsNoTracking().ToList();

        if (veiculos is null)
        {
            return NoContent();
        }

        return Ok(veiculos);
    }

    /* Talvez transformar em um IQueryble*/
    [HttpGet("{id:int:min(1)}", Name = "ObterVeiculo")]
    public ActionResult<Veiculo> Get(int id)
    {
        var veiculo = _uof.VeiculoRepository.Get(v => v.VeiculoId == id);

        if (veiculo is null)
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

        if(veiculo.Estado == Veiculo.EstadoVeiculo.Manutencao)
        {
            /* Todas as reservas desse veículo devem ser canceladas --> criar uma função pra retornar todas as reservas de um veículo */
            /* Cada reserva é cancelada */
        }

        var veiculoAtualizado = _uof.VeiculoRepository.Update(veiculo);
        _uof.Commit();

        return Ok(veiculo);
    }

    [HttpDelete("{id:int:min(1)}")] 
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

        /* Todas as reservas desse veículo devem ser canceladas --> criar uma rota pra retornar todas as reservas de um veículo */
        /* Cada reserva é cancelada */

        existeVeiculo.Estado = (Veiculo.EstadoVeiculo) 2; //Indisponível

        var veiculoDeletado = _uof.VeiculoRepository.Delete(existeVeiculo);
        _uof.Commit();

        return Ok(veiculoDeletado);
    }
}

