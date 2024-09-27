using ApiVeiculos.Context;
using ApiVeiculos.Models;
using static ApiVeiculos.Models.Veiculo;

namespace ApiVeiculos.Repositories;

public class VeiculoRepository : Repository<Veiculo>, IVeiculoRepository
{
    public VeiculoRepository(AppDbContext context) : base(context) { }

    public IQueryable<Veiculo> GetVeiculosDisponiveis()
    {
        return _context.Veiculos.Where(v => v.Estado != EstadoVeiculo.Manutencao && v.Estado != EstadoVeiculo.Indisponivel);
    }
}

