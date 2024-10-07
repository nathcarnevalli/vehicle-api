using ApiVeiculos.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace ApiVeiculos.Context;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Veiculo> Veiculos { get; set; }    
    public DbSet<Reserva> Produtos { get; set; }
}
