namespace GAC.Integration.FileIntegration.Models
{
    public class PurchaseOrder
    {
        public Guid Id { get; set; }
        public DateTime ProcessingDate { get; set; }
        public Guid CustomerId { get; set; }
        public List<PurchaseOrderLine> Lines { get; set; } = new();
    }
}
