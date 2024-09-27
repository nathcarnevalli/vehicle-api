using ApiVeiculos.Context;
using ApiVeiculos.Models;
namespace ApiVeiculos.Repositories;

public class ReservaRepository : Repository<Reserva>, IReservaRepository
{
    public ReservaRepository(AppDbContext context) : base(context) { }
}

