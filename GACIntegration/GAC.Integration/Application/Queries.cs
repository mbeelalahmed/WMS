using GAC.Integration.Domain.Entities;
using MediatR;

namespace GAC.Integration.Application.Queries
{
    public record GetAllCustomersQuery() : IRequest<List<Customer>>;
    public record GetAllProductsQuery() : IRequest<List<Product>>;
    public record GetAllSalesOrdersQuery() : IRequest<List<SalesOrder>>;
    public record GetAllPurchaseOrdersQuery() : IRequest<List<PurchaseOrder>>;
}
