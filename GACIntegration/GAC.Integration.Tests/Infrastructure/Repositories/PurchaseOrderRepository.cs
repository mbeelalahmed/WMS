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

        [Fact]
        public async Task GetByIdAsync_Should_Return_PurchaseOrder_With_Lines()
        {
            var context = CreateDbContext();
            var id = Guid.NewGuid();

            context.PurchaseOrders.Add(new PurchaseOrder
            {
                Id = id,
                ProcessingDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                Lines = new List<PurchaseOrderItem>
                {
                    new PurchaseOrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = Guid.NewGuid(),
                        Quantity = 1
                    }
                }
            });
            await context.SaveChangesAsync();

            var repository = new PurchaseOrderRepository(context);
            var result = await repository.GetByIdAsync(id);

            result.Should().NotBeNull();
            result.Id.Should().Be(id);
            result.Lines.Should().HaveCount(1);
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_PurchaseOrder_And_Lines()
        {
            var context = CreateDbContext();
            var id = Guid.NewGuid();

            var po = new PurchaseOrder
            {
                Id = id,
                ProcessingDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                Lines = new List<PurchaseOrderItem>
                {
                    new PurchaseOrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = Guid.NewGuid(),
                        Quantity = 3
                    }
                }
            };

            context.PurchaseOrders.Add(po);
            await context.SaveChangesAsync();

            po.ProcessingDate = po.ProcessingDate.AddDays(2);
            po.Lines[0].Quantity = 10;

            var repository = new PurchaseOrderRepository(context);
            await repository.UpdateAsync(po);

            var updated = await context.PurchaseOrders.Include(p => p.Lines).FirstOrDefaultAsync(p => p.Id == id);
            updated.ProcessingDate.Date.Should().Be(po.ProcessingDate.Date);
            updated.Lines[0].Quantity.Should().Be(10);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_PurchaseOrder()
        {
            var context = CreateDbContext();
            var id = Guid.NewGuid();

            var po = new PurchaseOrder
            {
                Id = id,
                ProcessingDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                Lines = new List<PurchaseOrderItem>
                {
                    new PurchaseOrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = Guid.NewGuid(),
                        Quantity = 6
                    }
                }
            };

            context.PurchaseOrders.Add(po);
            await context.SaveChangesAsync();

            var repository = new PurchaseOrderRepository(context);
            await repository.DeleteAsync(id);

            var deleted = await context.PurchaseOrders.FindAsync(id);
            deleted.Should().BeNull();
        }
    }
}
