using ApiVeiculos.Context;
using ApiVeiculos.Models;
using ApiVeiculos.Pagination;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
namespace ApiVeiculos.Repositories;

public class ReservaRepository : Repository<Reserva>, IReservaRepository
{
    public ReservaRepository(AppDbContext context) : base(context) { }

    public async Task<IPagedList<Reserva>> GetReservasAsync(QueryStringParameters parameters)
    {
        var reservas = await GetAllAsync();

        var reservasOrdenadas = reservas.OrderBy(r => r.ReservaId).AsQueryable();

        return reservasOrdenadas.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<IPagedList<Reserva>>? GetReservasVeiculoAsync(int id, QueryStringParameters parameters)
    {
        var veiculos = await _context.Veiculos
            .Include(v => v.Reservas).AsNoTracking().FirstOrDefaultAsync(v => v.VeiculoId == id);

        var veiculoReservas = veiculos.Reservas.OrderBy(r => r.ReservaId).AsQueryable();

        return veiculoReservas.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<IEnumerable<Reserva>>? GetAllReservasVeiculoAsync(int id)
    {
        var veiculos = await _context.Veiculos
            .Include(v => v.Reservas).AsNoTracking().FirstOrDefaultAsync(v => v.VeiculoId == id);

        return veiculos.Reservas.ToList();
    }
}

