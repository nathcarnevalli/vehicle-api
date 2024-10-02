using ApiVeiculos.Models;

namespace ApiVeiculos.Repositories;
public interface IReservaRepository : IRepository<Reserva>
{
    public IEnumerable<Reserva>? GetReservasVeiculo(int id);
}
