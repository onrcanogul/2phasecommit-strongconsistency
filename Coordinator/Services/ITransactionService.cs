namespace Coordinator.Services
{
    public interface ITransactionService
    {
        Task<Guid> CreateTransactionAsync();
        Task PrepareServicesAsync(Guid transactionId);
        Task<bool> CheckReadyServices(Guid transactionId);
        Task Commit(Guid transactionId);
        Task<bool> CheckTransactionStateServices(Guid transactionId);
        Task Rollback(Guid transactionId);

    }
}
