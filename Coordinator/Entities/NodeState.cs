using Coordinator.Enums;

namespace Coordinator.Entities
{
    public record NodeState(Guid TransactionId)
    {
        public Guid Id { get; set; }
        public TransactionState TransactionState { get; set; }
        public ReadyType IsReady { get; set; }
        public Node Node { get; set; }

    }
}
