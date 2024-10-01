using ApiVeiculos.Models;

namespace ApiVeiculos.Repositories;

public interface IVeiculoRepository : IRepository<Veiculo>
{
    IQueryable<Veiculo> GetVeiculosDisponiveis(DateTime dataInicio, DateTime dataFim);
}

