using FluentAssertions;
using GAC.Integration.Domain.Entities;
using GAC.Integration.Infrastructure.Data;
using GAC.Integration.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAC.Integration.Tests.Infrastructure.Repositories
{
    public class SalesOrderRepositoryTests
    {
        private async Task<AppDbContext> CreateDbContextAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);
            await context.Database.EnsureCreatedAsync();
            return context;
        }

        [Fact]
        public async Task AddAsync_Should_Add_SalesOrder_To_Database()
        {
            var context = await CreateDbContextAsync();
            var repository = new SalesOrderRepository(context);
            var salesOrder = new SalesOrder
            {
                Id = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                ProcessingDate = DateTime.UtcNow,
                ShipmentAddress = "123 Street",
                Lines = new List<SalesOrderItem>
                {
                    new SalesOrderItem { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2 }
                }
            };

            await repository.AddAsync(salesOrder);

            var stored = await context.SalesOrders.Include(s => s.Lines).FirstOrDefaultAsync(s => s.Id == salesOrder.Id);
            stored.Should().NotBeNull();
            stored.CustomerId.Should().Be(salesOrder.CustomerId);
            stored.ShipmentAddress.Should().Be("123 Street");
            stored.Lines.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_SalesOrders()
        {
            var context = await CreateDbContextAsync();
            var salesOrders = new List<SalesOrder>
            {
                new SalesOrder { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid(), ProcessingDate = DateTime.UtcNow, ShipmentAddress = "A" },
                new SalesOrder { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid(), ProcessingDate = DateTime.UtcNow, ShipmentAddress = "B" }
            };

            context.SalesOrders.AddRange(salesOrders);
            await context.SaveChangesAsync();

            var repository = new SalesOrderRepository(context);

            var result = await repository.GetAllAsync();

            result.Should().HaveCount(2);
        }
    }
}
