using ApiVeiculos.Context;
using ApiVeiculos.Models;
using Microsoft.EntityFrameworkCore;
using static ApiVeiculos.Models.Reserva;
using static ApiVeiculos.Models.Veiculo;

namespace ApiVeiculos.Repositories;

public class VeiculoRepository : Repository<Veiculo>, IVeiculoRepository
{
    public VeiculoRepository(AppDbContext context) : base(context) { }

    public IQueryable<Veiculo> GetVeiculosDisponiveis(DateTime dataInicio, DateTime dataFim)
    {
        return _context.Veiculos
            .Include(v => v.Reservas)
            .Where(v => v.Estado.Equals(EstadoVeiculo.Disponivel) && (!v.Reservas.Any(r =>
                r.DataInicio < dataInicio && r.DataFim < dataFim) || v.Reservas.Count == 0 || v.Reservas.All(r => r.Estado.Equals(EstadoReserva.Cancelado))));
    }

}

