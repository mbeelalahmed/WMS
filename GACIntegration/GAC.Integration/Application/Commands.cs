using MediatR;
using System.Collections.Generic;

namespace GAC.Integration.Application.Commands
{
    public record CreateCustomerCommand(string Name, string Address) : IRequest<Guid>;
    public record CreateProductCommand(string Code, string Title, string Description, string Dimensions) : IRequest<Guid>;
    public record CreatePurchaseOrderCommand(DateTime ProcessingDate, Guid CustomerId, List<PurchaseOrderLineDto> Lines) : IRequest<Guid>;
    public record CreateSalesOrderCommand(DateTime ProcessingDate, string ShipmentAddress, Guid CustomerId, List<SalesOrderLineDto> Lines) : IRequest<Guid>;

    public record PurchaseOrderLineDto(Guid ProductId, int Quantity);
    public record SalesOrderLineDto(Guid ProductId, int Quantity);
}
