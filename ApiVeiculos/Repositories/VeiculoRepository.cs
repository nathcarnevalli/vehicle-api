using ApiVeiculos.Context;
using ApiVeiculos.Models;
using Microsoft.EntityFrameworkCore;
using static ApiVeiculos.Models.Reserva;
using static ApiVeiculos.Models.Veiculo;

namespace ApiVeiculos.Repositories;

public class VeiculoRepository : Repository<Veiculo>, IVeiculoRepository
{
    public VeiculoRepository(AppDbContext context) : base(context) { }

    public async Task<IQueryable<Veiculo>> GetVeiculosDisponiveisAsync(DateTime dataInicio, DateTime dataFim)
    {
        var veiculos = await _context.Veiculos
            .Include(v => v.Reservas)
            .AsNoTracking().Where(v => v.Estado.Equals(EstadoVeiculo.Disponivel) && (!v.Reservas.Any(r =>
                r.DataInicio <= dataInicio && r.DataFim >= dataFim) || v.Reservas.Count == 0 || v.Reservas.All(r => r.Estado.Equals(EstadoReserva.Cancelado)))).ToListAsync();

        return veiculos.AsQueryable();
    }

}

