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
    public class PurchaseOrderRepositoryTests
    {
        private static AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_Should_Add_PurchaseOrder_With_Lines()
        {
            var context = CreateDbContext();
            var repository = new PurchaseOrderRepository(context);

            var po = new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                ProcessingDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                Lines = new List<PurchaseOrderItem>
                {
                    new PurchaseOrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = Guid.NewGuid(),
                        Quantity = 5
                    }
                }
            };

            await repository.AddAsync(po);

            var added = await context.PurchaseOrders.Include(p => p.Lines).FirstOrDefaultAsync();
            added.Should().NotBeNull();
            added.Lines.Should().HaveCount(1);
            added.Lines[0].Quantity.Should().Be(5);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_PurchaseOrders_With_Lines()
        {
            var context = CreateDbContext();
            context.PurchaseOrders.Add(new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                ProcessingDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                Lines = new List<PurchaseOrderItem>
                {
                    new PurchaseOrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = Guid.NewGuid(),
                        Quantity = 2
                    },
                    new PurchaseOrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = Guid.NewGuid(),
                        Quantity = 4
                    }
                }
            });
            await context.SaveChangesAsync();

            var repository = new PurchaseOrderRepository(context);

            var result = await repository.GetAllAsync();

            result.Should().HaveCount(1);
            result[0].Lines.Should().HaveCount(2);
        }
    }
}
