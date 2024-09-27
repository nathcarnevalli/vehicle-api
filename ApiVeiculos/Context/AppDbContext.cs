using ApiVeiculos.Models;
using Microsoft.EntityFrameworkCore;
namespace ApiVeiculos.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Veiculo> Veiculos { get; set; }    
    public DbSet<Reserva> Produtos { get; set; }
}
