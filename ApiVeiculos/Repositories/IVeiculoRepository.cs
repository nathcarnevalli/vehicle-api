using ApiVeiculos.Models;
using ApiVeiculos.Pagination;
using X.PagedList;

namespace ApiVeiculos.Repositories;

public interface IVeiculoRepository : IRepository<Veiculo>
{
    Task<IPagedList<Veiculo>> GetVeiculosAsync(QueryStringParameters parameters);
    Task<IPagedList<Veiculo>> GetVeiculosDisponiveisAsync(DateTime dataInicio, DateTime dataFim, QueryStringParameters parameters);
    Task<Veiculo> GetVeiculoDisponivelByIdAsync(DateTime dataInicio, DateTime dataFim, int id);
    bool ValidaPlaca(string placa);
}

