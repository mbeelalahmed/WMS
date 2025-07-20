namespace GAC.Integration.Domain.Entities
{
    public class PurchaseOrder
    {
        public Guid Id { get; set; }
        public DateTime ProcessingDate { get; set; }
        public Guid CustomerId { get; set; }
        public List<PurchaseOrderItem> Lines { get; set; } = new();
    }
}
