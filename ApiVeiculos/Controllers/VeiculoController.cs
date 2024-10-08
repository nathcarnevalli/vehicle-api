using ApiVeiculos.Models;
using ApiVeiculos.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiVeiculos.Controllers;

[ApiController]
[Route("[controller]")]

public class VeiculosController : ControllerBase
{
    private readonly IUnitOfWork _uof;

    public VeiculosController(IUnitOfWork uof)
    {
        _uof = uof;
    }

    [HttpGet]
    [Authorize(Policy = "GerenteOnly")]
    public ActionResult<IEnumerable<Veiculo>> Get()
    {
        var veiculos = _uof.VeiculoRepository.GetAll();

        if(!veiculos.Any())
        {
            return NoContent();
        }

        return Ok(new { Status = "200", Data = veiculos }); 
    }

    [HttpGet("Disponiveis")]
    [Authorize(Policy = "AllRoles")]
    public ActionResult<IEnumerable<Veiculo>> GetVeiculosDisponiveis(DateTime dataInicio, DateTime dataFim)
    {
        if (dataInicio >= dataFim)
        {
            return BadRequest(new { Status = "400", Message = "Datas inválidas" });
        }

        var veiculos = _uof.VeiculoRepository.GetVeiculosDisponiveis(dataInicio, dataFim).AsNoTracking().ToList();

        if (veiculos.Count == 0)
        {
            return NoContent();
        }

        return Ok(new { Status = "200", Data = veiculos });
    }

    [HttpGet("{id:int:min(1)}/Reservas")]
    [Authorize(Policy = "GerenteOnly")]
    public ActionResult<IEnumerable<Reserva>> GetReservas(int id)
    {
        var reservas = _uof.ReservaRepository.GetReservasVeiculo(id);

        if (!reservas.Any())
        {
            return NoContent();
        }

        return Ok(new { Status = "200", Data = reservas });
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterVeiculo")]
    [Authorize(Policy = "GerenteOnly")]
    public ActionResult<Veiculo> Get(int id)
    {
        var veiculo = _uof.VeiculoRepository.Get(v => v.VeiculoId == id);

        if (veiculo is null)
        {
            return NotFound(new { Status = "404", Message = "Veículo não encontrado"});
        }

        return Ok(new { Status = "200", Data = veiculo });
    }

    [HttpPost]
    [Authorize(Policy = "FuncionarioOrGerenteOnly")]
    public ActionResult<Veiculo> Post(Veiculo veiculo)
    {
        var existeVeiculo = _uof.VeiculoRepository.Get(v => v.Placa == veiculo.Placa);

        if(existeVeiculo is not null)
        {
            return Conflict($"Veículo {veiculo.Modelo} de placa {veiculo.Placa} já existe"); 
        }

        var veiculoCriado = _uof.VeiculoRepository.Create(veiculo);
        _uof.Commit();

        return new CreatedAtRouteResult("ObterVeiculo", new { Status = "201", Data = veiculoCriado, Message = "Veículo criado com sucesso" }); 
    }

    [HttpPut("{id:int:min(1)}")]
    [Authorize(Policy = "GerenteOnly")]
    public ActionResult<Veiculo> Put(Veiculo veiculo, int id) 
    {
        var existeVeiculo = _uof.VeiculoRepository.Get(v => v.VeiculoId == id);

        if(existeVeiculo is null)
        {
            return NotFound(new { Status = "404", Message = $"Veículo {veiculo.Modelo} não encontrado" });
        }

        if (veiculo.Estado.Equals(Veiculo.EstadoVeiculo.Manutencao))
        {
            var reservas = _uof.ReservaRepository.GetReservasVeiculo(id);

            foreach (var reserva in reservas!)
            {
                if (reserva.Estado.Equals(Reserva.EstadoReserva.Provisorio) || reserva.Estado.Equals(Reserva.EstadoReserva.Confirmado))
                {
                    reserva.Estado = Reserva.EstadoReserva.Cancelado;
                    _uof.ReservaRepository.Delete(reserva);
                }
            }
        }

        var veiculoAtualizado = _uof.VeiculoRepository.Update(veiculo);
        _uof.Commit();

        return Ok(new { Status = "200", Data = veiculo, Message = "Veículo atualizado com sucesso" });
    }

    [HttpDelete("{id:int:min(1)}")]
    [Authorize(Policy = "GerenteOnly")]
    public ActionResult<Veiculo> Delete(int id)
    {
        var existeVeiculo = _uof.VeiculoRepository.Get(v => v.VeiculoId == id);

        if(existeVeiculo is null)
        {
            return NotFound(new { Status = "404", Message = "Veículo não encontrado" });
        }else if (existeVeiculo.Estado.Equals(Reserva.EstadoReserva.Cancelado))
        {
            return BadRequest(new { Status = "404", Message = $"Veículo {existeVeiculo.Modelo} já foi deletado"});
        }

        var reservas = _uof.ReservaRepository.GetReservasVeiculo(id);

        foreach (var reserva in reservas!)
        {
            if (reserva.Estado.Equals(Reserva.EstadoReserva.Provisorio) || reserva.Estado.Equals(Reserva.EstadoReserva.Confirmado))
            {
                reserva.Estado = Reserva.EstadoReserva.Cancelado;
                _uof.ReservaRepository.Delete(reserva);
            }
        }

        existeVeiculo.Estado = (Veiculo.EstadoVeiculo) 2; //Indisponível

        var veiculoDeletado = _uof.VeiculoRepository.Delete(existeVeiculo);
        _uof.Commit();

        return Ok(new { Status = "200", Data = veiculoDeletado, Message = $"Veículo {existeVeiculo.Modelo} deletado com sucesso" });
    }
}

