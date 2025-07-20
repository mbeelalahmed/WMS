using GAC.Integration.Domain.Entities;
using MediatR;

namespace GAC.Integration.Application.Queries
{
    public record GetAllCustomersQuery() : IRequest<List<Customer>>;
    public record GetCustomerByIdQuery(Guid Id) : IRequest<Customer>;

    public record GetAllProductsQuery() : IRequest<List<Product>>;
    public record GetProductByIdQuery(Guid Id) : IRequest<Product?>;

    public record GetAllSalesOrdersQuery() : IRequest<List<SalesOrder>>;
    public record GetSalesOrderByIdQuery(Guid Id) : IRequest<SalesOrder>;

    public record GetAllPurchaseOrdersQuery() : IRequest<List<PurchaseOrder>>;
    public record GetPurchaseOrderByIdQuery(Guid Id) : IRequest<PurchaseOrder?>;

}
