using GAC.Integration.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace GAC.Integration.Application.Commands
{
    public record CreateCustomerCommand(string Name, string Address) : IRequest<Guid>;
    public record UpdateCustomerCommand(Guid Id, string Name, string Address) : IRequest<bool>;
    public record DeleteCustomerCommand(Guid Id) : IRequest<bool>;
    public record CreateProductCommand(string Code, string Title, string Description, string Dimensions) : IRequest<Guid>;
    public record UpdateProductCommand : IRequest<bool>
    {
        public Guid Id { get; init; }
        public string Code { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public string Dimensions { get; init; }
    }

    public record DeleteProductCommand(Guid Id) : IRequest<bool>;
    public record CreatePurchaseOrderCommand(DateTime ProcessingDate, Guid CustomerId, List<PurchaseOrderLineDto> Lines) : IRequest<Guid>;
    public class UpdatePurchaseOrderCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public DateTime ProcessingDate { get; set; }
        public Guid CustomerId { get; set; }
        public List<PurchaseOrderItem> Lines { get; set; } = new();
    }
    public record DeletePurchaseOrderCommand(Guid Id) : IRequest<bool>;

    public record CreateSalesOrderCommand(DateTime ProcessingDate, string ShipmentAddress, Guid CustomerId, List<SalesOrderLineDto> Lines) : IRequest<Guid>;

    public class UpdateSalesOrderCommand : IRequest<bool>
    {
        public Guid Id { get; init; }
        public DateTime ProcessingDate { get; init; }
        public string ShipmentAddress { get; init; } = string.Empty;
        public Guid CustomerId { get; init; }
        public List<SalesOrderItem> Lines { get; init; } = new();
    }
    public record DeleteSalesOrderCommand(Guid Id) : IRequest<bool>;

    public record PurchaseOrderLineDto(Guid ProductId, int Quantity);
    public record SalesOrderLineDto(Guid ProductId, int Quantity);
}
