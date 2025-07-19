using GAC.Integration.Application.Commands;
using GAC.Integration.Application.Queries;
using GAC.Integration.Domain.Entities;
using GAC.Integration.Domain.Interfaces;
using MediatR;

namespace GAC.Integration.Application.Handler
{
    public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, Guid>
    {
        private readonly ICustomerRepository _repo;

        public CreateCustomerHandler(ICustomerRepository repo) => _repo = repo;

        public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = new Customer { Id = Guid.NewGuid(), Name = request.Name, Address = request.Address };
            await _repo.AddAsync(customer);
            return customer.Id;
        }
    }

    public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _repo;

        public CreateProductHandler(IProductRepository repo) => _repo = repo;

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Code = request.Code,
                Title = request.Title,
                Description = request.Description,
                Dimensions = request.Dimensions
            };
            await _repo.AddAsync(product);
            return product.Id;
        }
    }

    public class CreatePurchaseOrderHandler : IRequestHandler<CreatePurchaseOrderCommand, Guid>
    {
        private readonly IPurchaseOrderRepository _repo;

        public CreatePurchaseOrderHandler(IPurchaseOrderRepository repo) => _repo = repo;

        public async Task<Guid> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            var po = new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                ProcessingDate = request.ProcessingDate,
                CustomerId = request.CustomerId,
                Lines = request.Lines.Select(l => new PurchaseOrderLine { Id = Guid.NewGuid(), ProductId = l.ProductId, Quantity = l.Quantity }).ToList()
            };
            await _repo.AddAsync(po);
            return po.Id;
        }
    }

    public class CreateSalesOrderHandler : IRequestHandler<CreateSalesOrderCommand, Guid>
    {
        private readonly ISalesOrderRepository _repo;

        public CreateSalesOrderHandler(ISalesOrderRepository repo) => _repo = repo;

        public async Task<Guid> Handle(CreateSalesOrderCommand request, CancellationToken cancellationToken)
        {
            var so = new SalesOrder
            {
                Id = Guid.NewGuid(),
                ProcessingDate = request.ProcessingDate,
                ShipmentAddress = request.ShipmentAddress,
                CustomerId = request.CustomerId,
                Lines = request.Lines.Select(l => new SalesOrderLine { Id = Guid.NewGuid(), ProductId = l.ProductId, Quantity = l.Quantity }).ToList()
            };
            await _repo.AddAsync(so);
            return so.Id;
        }
    }

    /* Queries handler */

    public class GetAllCustomersHandler : IRequestHandler<GetAllCustomersQuery, List<Customer>>
    {
        private readonly ICustomerRepository _repo;

        public GetAllCustomersHandler(ICustomerRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Customer>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync();
        }

    }

    public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, List<Product>>
    {
        private readonly IProductRepository _repo;

        public GetAllProductsHandler(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync();
        }

    }


    public class GetAllSalesOrdersHandler : IRequestHandler<GetAllSalesOrdersQuery, List<SalesOrder>>
    {
        private readonly ISalesOrderRepository _repo;

        public GetAllSalesOrdersHandler(ISalesOrderRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<SalesOrder>> Handle(GetAllSalesOrdersQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync();
        }
    }

    public class GetAllPurchaseOrdersHandler : IRequestHandler<GetAllPurchaseOrdersQuery, List<PurchaseOrder>>
    {
        private readonly IPurchaseOrderRepository _repo;

        public GetAllPurchaseOrdersHandler(IPurchaseOrderRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<PurchaseOrder>> Handle(GetAllPurchaseOrdersQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync();
        }
    }

}
