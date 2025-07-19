namespace GAC.Integration.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Dimensions { get; set; }
    }
}
