using ApiVeiculos.Context;
using ApiVeiculos.Models;
using Microsoft.EntityFrameworkCore;
using static ApiVeiculos.Models.Reserva;
using static ApiVeiculos.Models.Veiculo;

namespace ApiVeiculos.Repositories;

public class VeiculoRepository : Repository<Veiculo>, IVeiculoRepository
{
    public VeiculoRepository(AppDbContext context) : base(context) { }

    public IQueryable<Veiculo> GetVeiculosDisponiveis(string dataInicio, string dataFim)
    {
        DateTime inicio = DateTime.Parse(dataInicio);
        DateTime fim = DateTime.Parse(dataFim);

        return _context.Veiculos
            .Include(v => v.Reservas)
            .Where(v => v.Estado != EstadoVeiculo.Manutencao || v.Estado != EstadoVeiculo.Indisponivel)
            .Where(v => v.Reservas.Any(r =>
                (r.DataInicio > inicio && r.DataFim > inicio) || (r.DataInicio < fim && r.DataFim < fim) || r.Estado == EstadoReserva.Cancelado) || v.Reservas.Count == 0);
    }

}

