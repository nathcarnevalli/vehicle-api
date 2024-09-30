using ApiVeiculos.Models;

namespace ApiVeiculos.Repositories;

public interface IVeiculoRepository : IRepository<Veiculo>
{
    IQueryable<Veiculo> GetVeiculosDisponiveis(string dataInicio, string dataFim);
}

