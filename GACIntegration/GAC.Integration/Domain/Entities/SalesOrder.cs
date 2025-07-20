namespace GAC.Integration.Domain.Entities
{
    public class SalesOrder
    {
        public Guid Id { get; set; }
        public DateTime ProcessingDate { get; set; }
        public string ShipmentAddress { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public List<SalesOrderItem> Lines { get; set; } = new();
    }
}
