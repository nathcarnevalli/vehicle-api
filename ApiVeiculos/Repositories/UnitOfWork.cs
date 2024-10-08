using ApiVeiculos.Context;

namespace ApiVeiculos.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private IVeiculoRepository? _veiculoRepository;
        private IReservaRepository? _reservaRepository;
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IVeiculoRepository VeiculoRepository
        {
            get { return _veiculoRepository = _veiculoRepository ?? new VeiculoRepository(_context);  }
        }

        public IReservaRepository ReservaRepository
        {
            get { return _reservaRepository = _reservaRepository ?? new ReservaRepository(_context); } 
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task Dispose() 
        {
            await _context.DisposeAsync();
        }
    }
}
