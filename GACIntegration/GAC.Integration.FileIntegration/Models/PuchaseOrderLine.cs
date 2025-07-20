namespace GAC.Integration.FileIntegration.Models
{
    public class PurchaseOrderLine
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
