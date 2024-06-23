namespace Coordinator.Entities
{
    public record Node(string Name)
    {
        public Guid Id { get; set; }
        public ICollection<NodeState> NodeStates { get; set; }
    }
}
