using ApiVeiculos.Context;
using ApiVeiculos.Models;
using Microsoft.EntityFrameworkCore;
namespace ApiVeiculos.Repositories;

public class ReservaRepository : Repository<Reserva>, IReservaRepository
{
    public ReservaRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Reserva>>? GetReservasVeiculoAsync(int id)
    {
        var veiculos = await _context.Veiculos
            .Include(v => v.Reservas).AsNoTracking().FirstOrDefaultAsync(v => v.VeiculoId == id);

        return veiculos.Reservas.ToList();
    }
}

