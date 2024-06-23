
using Coordinator.Contexts;
using Coordinator.Entities;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Coordinator.Services
{
    public class TransactionService(AppDbContext context, IHttpClientFactory httpClientFactory) : ITransactionService
    {

        HttpClient httpClientOrder = httpClientFactory.CreateClient("OrderAPI");
        HttpClient httpClientStock = httpClientFactory.CreateClient("StockAPI");
        HttpClient httpClientPayment = httpClientFactory.CreateClient("PaymentAPI");
        public async Task<Guid> CreateTransactionAsync()
        {
            Guid transactionId = Guid.NewGuid();
            var nodes = await context.Nodes.ToListAsync();
            nodes.ForEach(n => n.NodeStates = new List<NodeState>
            {
                new(transactionId)
                {
                    TransactionState = Enums.TransactionState.Pending,
                    IsReady = Enums.ReadyType.Pending
                }
            });

            await context.SaveChangesAsync();
            return transactionId;
        }
        public async Task PrepareServicesAsync(Guid transactionId)
        {
           var nodeStates =  await context.NodeStates
                .Include(x => x.Node)
                .Where(x => x.TransactionId == transactionId)
                .ToListAsync();

            foreach (var transactionNode in nodeStates) 
            {
                try
                {
                    var response = await (transactionNode.Node.Name switch
                    {
                        "Order.API" => httpClientOrder.GetAsync("/ready"),
                        "Stock.API" => httpClientStock.GetAsync("/ready"),
                        "Payment.API" => httpClientPayment.GetAsync("/ready")
                    });

                    bool result = bool.Parse(await response.Content.ReadAsStringAsync());
                    transactionNode.IsReady = result ? Enums.ReadyType.Ready : Enums.ReadyType.Unready;
                }
                catch (Exception ex)
                {
                    transactionNode.IsReady = Enums.ReadyType.Unready;
                }
            }
            await context.SaveChangesAsync();

        }
        public async Task<bool> CheckReadyServices(Guid transactionId)
        {
            var nodeStates = await context.NodeStates.Where(x => x.TransactionId == transactionId).ToListAsync();

            return nodeStates.TrueForAll(x => x.IsReady == Enums.ReadyType.Ready);
        }
        public async Task Commit(Guid transactionId)
        {
            var nodeStates = await context.NodeStates
                .Include(x => x.Node)
                .Where(x => x.TransactionId == transactionId)
                .ToListAsync();

            foreach (var transactionNode in nodeStates)
            {
                try
                {
                    var response = await (transactionNode.Node.Name switch
                    {
                        "Order.API" => httpClientOrder.GetAsync("/commit"),
                        "Stock.API" => httpClientStock.GetAsync("/commit"),
                        "Payment.API" => httpClientPayment.GetAsync("/commit")
                    });
                    bool result = bool.Parse(await response.Content.ReadAsStringAsync());
                    transactionNode.TransactionState = result ? Enums.TransactionState.Done : Enums.TransactionState.Abort;
                }
                catch (Exception ex)
                {
                    transactionNode.TransactionState = Enums.TransactionState.Abort;
                }
            }
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckTransactionStateServices(Guid transactionId)
        {
            var nodeStates = await context.NodeStates
                .Where(x => x.TransactionId == transactionId)
                .ToListAsync();

            return nodeStates.TrueForAll(x => x.TransactionState == Enums.TransactionState.Done);
        }
        public async Task Rollback(Guid transactionId)
        {
            var nodeState = await context.NodeStates
                .Include(x => x.Node)
                .Where(x => x.TransactionId == transactionId)
                .ToListAsync();

            foreach (var transactionNode in nodeState)
            {
                try
                {
                    if (transactionNode.TransactionState == Enums.TransactionState.Done)
                    {
                        _ = transactionNode.Node.Name switch
                        {
                            "Order.API" => httpClientOrder.GetAsync("/rollback"),
                            "Stock.API" => httpClientStock.GetAsync("/rollback"),
                            "Payment.API" => httpClientPayment.GetAsync("/rollback")
                        };
                    }
                    transactionNode.TransactionState = Enums.TransactionState.Abort;
                }
                catch
                {
                    transactionNode.TransactionState = Enums.TransactionState.Abort;
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
