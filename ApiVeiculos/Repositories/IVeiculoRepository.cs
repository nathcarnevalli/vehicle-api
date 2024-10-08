using ApiVeiculos.Models;

namespace ApiVeiculos.Repositories;

public interface IVeiculoRepository : IRepository<Veiculo>
{
    Task<IQueryable<Veiculo>> GetVeiculosDisponiveisAsync(DateTime dataInicio, DateTime dataFim);
}

