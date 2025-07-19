using GAC.Integration.FileIntegration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GAC.Integration.FileIntegration.Services
{
    public class XmlFileParserService : IFileParserService
    {
        public async Task<IEnumerable<PurchaseOrder>> ParseFileAsync(string filePath)
        {
            var list = new List<PurchaseOrder>();
            var doc = await Task.Run(() => XDocument.Load(filePath));

            foreach (var order in doc.Descendants("PurchaseOrder"))
            {
                // Parse OrderId and CustomerId as Guid
                if (!Guid.TryParse(order.Element("OrderId")?.Value, out var orderId))
                    continue; // skip malformed
                if (!Guid.TryParse(order.Element("Customer")?.Value, out var customerId))
                    continue;

                // Parse date
                var processingDateString = order.Element("ProcessingDate")?.Value;
                var processingDate = DateTime.TryParse(processingDateString, out var dt)
                    ? dt
                    : DateTime.UtcNow;

                var purchaseOrder = new PurchaseOrder
                {
                    Id = orderId,
                    CustomerId = customerId,
                    ProcessingDate = processingDate
                };

                // Parse product lines
                foreach (var product in order.Descendants("Product"))
                {
                    if (!Guid.TryParse(product.Element("Key")?.Value, out var productId))
                        continue;
                    if (!int.TryParse(product.Element("Value")?.Value, out var quantity))
                        continue;

                    purchaseOrder.Lines.Add(new PurchaseOrderLine
                    {
                        ProductId = productId,
                        Quantity = quantity
                    });
                }

                list.Add(purchaseOrder);
            }

            return list;
        }

    }
}
