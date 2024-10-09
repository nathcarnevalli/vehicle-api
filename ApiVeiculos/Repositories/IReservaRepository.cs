using ApiVeiculos.Models;
using ApiVeiculos.Pagination;
using X.PagedList;

namespace ApiVeiculos.Repositories;
public interface IReservaRepository : IRepository<Reserva>
{
    public Task<IPagedList<Reserva>> GetReservasAsync(QueryStringParameters parameters);
    public Task<IPagedList<Reserva>>? GetReservasVeiculoAsync(int id, QueryStringParameters parameters);
    public Task<IEnumerable<Reserva>>? GetAllReservasVeiculoAsync(int id);
}
