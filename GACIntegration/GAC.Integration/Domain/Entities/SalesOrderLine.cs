namespace GAC.Integration.Domain.Entities
{
    public class SalesOrderLine
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
