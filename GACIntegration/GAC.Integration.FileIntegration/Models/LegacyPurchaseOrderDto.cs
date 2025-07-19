namespace GAC.Integration.FileIntegration.Models;

public class LegacyPurchaseOrderDto
{
    public string OrderId { get; set; }
    public DateTime ProcessingDate { get; set; }
    public string Customer { get; set; }
    public Dictionary<string, int> Products { get; set; } = new();
}
