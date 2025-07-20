using GAC.Integration.Application.Commands;
using GAC.Integration.Application.Queries;
using GAC.Integration.Domain.Entities;
using GAC.Integration.Domain.Interfaces;
using GAC.Integration.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

    public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, bool>
    {
        private readonly ICustomerRepository _repo;

        public UpdateCustomerHandler(ICustomerRepository repo) => _repo = repo;

        public async Task<bool> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _repo.GetByIdAsync(request.Id);
            if (customer == null) return false;

            customer.Name = request.Name;
            customer.Address = request.Address;

            await _repo.UpdateAsync(customer);
            return true;
        }
    }

    public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand, bool>
    {
        private readonly ICustomerRepository _repo;

        public DeleteCustomerHandler(ICustomerRepository repo) => _repo = repo;

        public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _repo.GetByIdAsync(request.Id);
            if (customer == null) return false;

            await _repo.DeleteAsync(customer);
            return true;
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

    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IProductRepository _repo;

        public UpdateProductHandler(IProductRepository repo) => _repo = repo;

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken ct)
        {
            var product = new Product
            {
                Id = request.Id,
                Code = request.Code,
                Title = request.Title,
                Description = request.Description,
                Dimensions = request.Dimensions
            };
            return await _repo.UpdateAsync(product);
        }
    }

    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductRepository _repo;

        public DeleteProductHandler(IProductRepository repo) => _repo = repo;

        public Task<bool> Handle(DeleteProductCommand request, CancellationToken ct) =>
            _repo.DeleteAsync(request.Id);
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
                Lines = request.Lines.Select(l => new PurchaseOrderItem { Id = Guid.NewGuid(), ProductId = l.ProductId, Quantity = l.Quantity }).ToList()
            };
            await _repo.AddAsync(po);
            return po.Id;
        }
    }

    public class UpdatePurchaseOrderHandler : IRequestHandler<UpdatePurchaseOrderCommand, bool>
    {
        private readonly IPurchaseOrderRepository _repo;
        public UpdatePurchaseOrderHandler(IPurchaseOrderRepository repo) => _repo = repo;

        public async Task<bool> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            var existing = await _repo.GetByIdAsync(request.Id);
            if (existing == null) return false;

            existing.ProcessingDate = request.ProcessingDate;
            existing.CustomerId = request.CustomerId;
            existing.Lines = request.Lines;

            await _repo.UpdateAsync(existing);
            return true;
        }
    }

    public class DeletePurchaseOrderHandler : IRequestHandler<DeletePurchaseOrderCommand, bool>
    {
        private readonly IPurchaseOrderRepository _repo;
        public DeletePurchaseOrderHandler(IPurchaseOrderRepository repo) => _repo = repo;

        public async Task<bool> Handle(DeletePurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            var existing = await _repo.GetByIdAsync(request.Id);
            if (existing == null) return false;

            await _repo.DeleteAsync(request.Id);
            return true;
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
                Lines = request.Lines.Select(l => new SalesOrderItem { Id = Guid.NewGuid(), ProductId = l.ProductId, Quantity = l.Quantity }).ToList()
            };
            await _repo.AddAsync(so);
            return so.Id;
        }
    }

    public class UpdateSalesOrderHandler : IRequestHandler<UpdateSalesOrderCommand, bool>
    {
        private readonly ISalesOrderRepository _repository;

        public UpdateSalesOrderHandler(ISalesOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateSalesOrderCommand request, CancellationToken cancellationToken)
        {
            var salesOrder = await _repository.GetByIdAsync(request.Id);
            if (salesOrder == null)
                return false;

            salesOrder.ProcessingDate = request.ProcessingDate;
            salesOrder.ShipmentAddress = request.ShipmentAddress;
            salesOrder.CustomerId = request.CustomerId;

            salesOrder.Lines.Clear();
            foreach (var line in request.Lines)
            {
                salesOrder.Lines.Add(line);
            }

            await _repository.UpdateAsync(salesOrder);
            return true;
        }
    }

    public class DeleteSalesOrderHandler : IRequestHandler<DeleteSalesOrderCommand, bool>
    {
        private readonly ISalesOrderRepository _repository;

        public DeleteSalesOrderHandler(ISalesOrderRepository repository) => _repository = repository;

        public async Task<bool> Handle(DeleteSalesOrderCommand request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteAsync(request.Id);
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
    public class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdQuery, Customer>
    {
        private readonly ICustomerRepository _repo;

        public GetCustomerByIdHandler(ICustomerRepository repo)
        {
            _repo = repo;
        }

        public async Task<Customer> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetByIdAsync(request.Id);
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

    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Product?>
    {
        private readonly IProductRepository _repo;

        public GetProductByIdHandler(IProductRepository repo) => _repo = repo;

        public Task<Product?> Handle(GetProductByIdQuery request, CancellationToken ct) =>
            _repo.GetByIdAsync(request.Id);
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

    public class GetSalesOrderByIdHandler : IRequestHandler<GetSalesOrderByIdQuery, SalesOrder>
    {
        private readonly AppDbContext _context;
        public GetSalesOrderByIdHandler(AppDbContext context) => _context = context;

        public async Task<SalesOrder> Handle(GetSalesOrderByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.SalesOrders
                .Include(so => so.Lines)
                .FirstOrDefaultAsync(so => so.Id == request.Id, cancellationToken);
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

    public class GetPurchaseOrderByIdHandler : IRequestHandler<GetPurchaseOrderByIdQuery, PurchaseOrder?>
    {
        private readonly IPurchaseOrderRepository _repo;
        public GetPurchaseOrderByIdHandler(IPurchaseOrderRepository repo) => _repo = repo;

        public Task<PurchaseOrder?> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
            => _repo.GetByIdAsync(request.Id);
    }

}
