using ApiVeiculos.Context;
using ApiVeiculos.Models;
using ApiVeiculos.Pagination;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using static ApiVeiculos.Models.Reserva;
using static ApiVeiculos.Models.Veiculo;

namespace ApiVeiculos.Repositories;

public class VeiculoRepository : Repository<Veiculo>, IVeiculoRepository
{
    public VeiculoRepository(AppDbContext context) : base(context) { }

    public async Task<IPagedList<Veiculo>> GetVeiculosAsync(QueryStringParameters parameters)
    {
        var veiculos = await GetAllAsync();

        var veiculosOrdenados = veiculos.OrderBy(v => v.VeiculoId).AsQueryable();

        return veiculosOrdenados.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<IPagedList<Veiculo>> GetVeiculosDisponiveisAsync(DateTime dataInicio, DateTime dataFim, QueryStringParameters parameters)
    {
        var veiculos = await _context.Veiculos
            .Include(v => v.Reservas)
            .AsNoTracking().Where(v => v.Estado.Equals(EstadoVeiculo.Disponivel) && (!v.Reservas.Any(r =>
                r.DataInicio <= dataInicio && r.DataFim >= dataFim) || v.Reservas.Count == 0 || v.Reservas.All(r => r.Estado.Equals(EstadoReserva.Cancelado)))).ToListAsync();

        var veiculosOrdenados = veiculos.OrderBy(v => v.VeiculoId).AsQueryable();

        return veiculosOrdenados.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<Veiculo> GetVeiculoDisponivelByIdAsync(DateTime dataInicio, DateTime dataFim, int id)
    {
        var veiculo = await _context.Veiculos
            .Include(v => v.Reservas)
            .AsNoTracking().Where(v => v.Estado.Equals(EstadoVeiculo.Disponivel) && (!v.Reservas.Any(r =>
                r.DataInicio <= dataInicio && r.DataFim >= dataFim) || v.Reservas.Count == 0 || v.Reservas.All(r => r.Estado.Equals(EstadoReserva.Cancelado))))
            .FirstOrDefaultAsync(v => v.VeiculoId == id);

        return veiculo!;
    }
}

