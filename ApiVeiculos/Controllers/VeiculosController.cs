using ApiVeiculos.Models;
using ApiVeiculos.Pagination;
using ApiVeiculos.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ApiVeiculos.Models.Reserva;
using static ApiVeiculos.Models.Veiculo;

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
    public async Task<ActionResult<IEnumerable<Veiculo>>> Get([FromQuery]QueryStringParameters parameters)
    {
        var veiculos = await _uof.VeiculoRepository.GetVeiculosAsync(parameters);

        if(!veiculos.Any())
        {
            return NoContent();
        }

        return Ok(new { Status = "200", Data = veiculos }); 
    }

    [HttpGet("Disponiveis")]
    [Authorize(Policy = "AllRoles")]
    public async Task<ActionResult<IEnumerable<Veiculo>>> GetVeiculosDisponiveis(DateTime dataInicio, DateTime dataFim, [FromQuery]QueryStringParameters parameters)
    {
        if (dataInicio >= dataFim)
        {
            return BadRequest(new { Status = "400", Message = "Datas inválidas" });
        }

        var veiculos = await _uof.VeiculoRepository.GetVeiculosDisponiveisAsync(dataInicio, dataFim, parameters);

        if (!veiculos.Any())
        {
            return NoContent();
        }

        return Ok(new { Status = "200", Data = veiculos });
    }

    [HttpGet("{id:int:min(1)}/Reservas")]
    [Authorize(Policy = "GerenteOnly")]
    public async Task<ActionResult<IEnumerable<Reserva>>> GetReservas(int id, [FromQuery]QueryStringParameters paramters)
    {
        var reservas = await _uof.ReservaRepository.GetReservasVeiculoAsync(id, paramters)!;

        if (!reservas.Any())
        {
            return NoContent();
        }

        return Ok(new { Status = "200", Data = reservas });
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterVeiculo")]
    [Authorize(Policy = "GerenteOnly")]
    public async Task<ActionResult<Veiculo>> Get(int id)
    {
        var veiculo = await _uof.VeiculoRepository.GetAsync(v => v.VeiculoId == id);

        if (veiculo is null)
        {
            return NotFound(new { Status = "404", Message = "Veículo não encontrado"});
        }

        return Ok(new { Status = "200", Data = veiculo });
    }

    [HttpPost]
    [Authorize(Policy = "FuncionarioOrGerenteOnly")]
    public async Task<ActionResult<Veiculo>> Post(Veiculo veiculo)
    {
        if (!_uof.VeiculoRepository.ValidaPlaca(veiculo.Placa!))
        {
            return BadRequest(new { Status = "400", Message = "Placa inválida"});
        }

        var existeVeiculo = await _uof.VeiculoRepository.GetAsync(v => v.Placa == veiculo.Placa);

        if(existeVeiculo is not null)
        {
            return Conflict(new { Status = "409", Message = $"Veículo de placa {veiculo.Placa} já existe"}); 
        }

        var veiculoCriado = _uof.VeiculoRepository.Create(veiculo);
        await _uof.CommitAsync();

        return new CreatedAtRouteResult("ObterVeiculo", new { Status = "201", Data = veiculoCriado, Message = "Veículo criado com sucesso" }); 
    }

    [HttpPut("{id:int:min(1)}")]
    [Authorize(Policy = "GerenteOnly")]
    public async Task<ActionResult<Veiculo>> Put(Veiculo veiculo, int id) 
    {
        if (id != veiculo.VeiculoId)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                   new { Status = "500", Message = "Houve um erro na alteração do veículo" });
        }

        if (!_uof.VeiculoRepository.ValidaPlaca(veiculo.Placa!))
        {
            return BadRequest(new { Status = "400", Message = "Placa inválida" });
        }

        var existeVeiculo = await _uof.VeiculoRepository.GetAsync(v => v.VeiculoId == id);

        if(existeVeiculo is null)
        {
            return NotFound(new { Status = "404", Message = $"Veículo {veiculo.Modelo} não encontrado" });
        }

        if(veiculo.Placa != existeVeiculo.Placa)
        {
            var existePlaca = await _uof.VeiculoRepository.GetAsync(v => v.Placa == veiculo.Placa);

            if(existePlaca is not null)
            {
                return Conflict(new { Status = "409", Message = $"Veículo de placa {veiculo.Placa} já existe"});
            }
        }

        if (veiculo.Estado.Equals(EstadoVeiculo.Manutencao))
        {
            var reservas = await _uof.ReservaRepository.GetAllReservasVeiculoAsync(id)!;

            foreach (var reserva in reservas!)
            {
                if (reserva.Estado.Equals(EstadoReserva.Provisorio) || reserva.Estado.Equals(EstadoReserva.Confirmado))
                {
                    reserva.Estado = EstadoReserva.Cancelado;
                    _uof.ReservaRepository.Delete(reserva);
                }
            }
        }

        var veiculoAtualizado = _uof.VeiculoRepository.Update(veiculo);
        await _uof.CommitAsync();

        return Ok(new { Status = "200", Data = veiculo, Message = $"Veículo {veiculo.Modelo} atualizado com sucesso" });
    }

    [HttpDelete("{id:int:min(1)}")]
    [Authorize(Policy = "GerenteOnly")]
    public async Task<ActionResult<Veiculo>> Delete(int id)
    {
        var existeVeiculo = await _uof.VeiculoRepository.GetAsync(v => v.VeiculoId == id);

        if(existeVeiculo is null)
        {
            return NotFound(new { Status = "404", Message = "Veículo não encontrado" });
        }else if (existeVeiculo.Estado.Equals(EstadoReserva.Cancelado))
        {
            return BadRequest(new { Status = "400", Message = $"Veículo {existeVeiculo.Modelo} já foi deletado"});
        }

        var reservas = await _uof.ReservaRepository.GetAllReservasVeiculoAsync(id)!;

        foreach (var reserva in reservas!)
        {
            if (reserva.Estado.Equals(EstadoReserva.Provisorio) || reserva.Estado.Equals(EstadoReserva.Confirmado))
            {
                reserva.Estado = EstadoReserva.Cancelado;
                _uof.ReservaRepository.Delete(reserva);
            }
        }

        existeVeiculo.Estado = EstadoVeiculo.Indisponivel; 

        var veiculoDeletado = _uof.VeiculoRepository.Delete(existeVeiculo);
        await _uof.CommitAsync();

        return Ok(new { Status = "200", Data = veiculoDeletado, Message = $"Veículo {existeVeiculo.Modelo} deletado com sucesso" });
    }
}

