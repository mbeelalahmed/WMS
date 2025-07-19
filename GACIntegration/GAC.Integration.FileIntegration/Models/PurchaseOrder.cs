using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAC.Integration.FileIntegration.Models
{
    public class PurchaseOrder
    {
        public Guid Id { get; set; }
        public DateTime ProcessingDate { get; set; }
        public Guid CustomerId { get; set; }
        public List<PurchaseOrderLine> Lines { get; set; } = new();

        public List<PurchaseOrder> MapToDomain(IEnumerable<LegacyPurchaseOrderDto> legacyDtos)
        {
            return legacyDtos.Select(dto => new PurchaseOrder
            {
                Id = Guid.Parse(dto.OrderId),
                ProcessingDate = dto.ProcessingDate,
                CustomerId = Guid.Parse(dto.Customer),
                Lines = dto.Products.Select(p => new PurchaseOrderLine
                {
                    ProductId = Guid.Parse(p.Key),
                    Quantity = p.Value
                }).ToList()
            }).ToList();
        }
    }
}
