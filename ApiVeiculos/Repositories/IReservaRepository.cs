using ApiVeiculos.Models;

namespace ApiVeiculos.Repositories;
public interface IReservaRepository : IRepository<Reserva>
{
    public Task<IEnumerable<Reserva>>? GetReservasVeiculoAsync(int id);
}
