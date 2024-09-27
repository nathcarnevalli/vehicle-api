namespace ApiVeiculos.Repositories
{
    public interface IUnitOfWork
    {
        public IVeiculoRepository VeiculoRepository { get; }

        public IReservaRepository ReservaRepository { get; }

        void Commit();

    }
}
