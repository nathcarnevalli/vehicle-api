using ApiVeiculos.Context;
using ApiVeiculos.Models;
using Microsoft.EntityFrameworkCore;
namespace ApiVeiculos.Repositories;

public class ReservaRepository : Repository<Reserva>, IReservaRepository
{
    public ReservaRepository(AppDbContext context) : base(context) { }

    public IEnumerable<Reserva>? GetReservasVeiculo(int id)
    {
        return _context.Veiculos
            .Where(v => v.VeiculoId == id)
            .Include(v => v.Reservas).FirstOrDefault(v => v.VeiculoId == id)?.Reservas;
    }
}

