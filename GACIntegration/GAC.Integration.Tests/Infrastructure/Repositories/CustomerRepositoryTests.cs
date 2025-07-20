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
    public class CustomerRepositoryTests
    {
        private static AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Customer_To_Database()
        {
            var context = CreateDbContext();
            var repository = new CustomerRepository(context);

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = "Test Customer",
                Address = "Dubai"
            };

            await repository.AddAsync(customer);

            var customers = await context.Customers.ToListAsync();
            customers.Should().ContainSingle();
            customers[0].Name.Should().Be("Test Customer");
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Customers()
        {
            var context = CreateDbContext();
            context.Customers.AddRange(
                new Customer { Id = Guid.NewGuid(), Name = "Customer A", Address = "dubai" },
                new Customer { Id = Guid.NewGuid(), Name = "Customer B", Address = "Tokyo" }
            );
            await context.SaveChangesAsync();

            var repository = new CustomerRepository(context);

            var result = await repository.GetAllAsync();

            result.Should().HaveCount(2);
            result.Should().Contain(c => c.Name == "Customer A");
            result.Should().Contain(c => c.Name == "Customer B");
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Customer_When_Found()
        {
            var context = CreateDbContext();
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = "Existing Customer",
                Address = "Abu Dhabi"
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            var repository = new CustomerRepository(context);

            var result = await repository.GetByIdAsync(customer.Id);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Existing Customer");
            result.Address.Should().Be("Abu Dhabi");
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Customer_Details()
        {
            var context = CreateDbContext();
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = "Original Name",
                Address = "Sharjah"
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            var repository = new CustomerRepository(context);

            customer.Name = "Updated Name";
            customer.Address = "Ajman";

            await repository.UpdateAsync(customer);

            var updated = await context.Customers.FindAsync(customer.Id);
            updated!.Name.Should().Be("Updated Name");
            updated.Address.Should().Be("Ajman");
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Customer_When_Exists()
        {
            var context = CreateDbContext();
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = "To Be Deleted",
                Address = "Fujairah"
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            var repository = new CustomerRepository(context);

            await repository.DeleteAsync(customer);

            var result = await context.Customers.FindAsync(customer.Id);
            result.Should().BeNull();
        }

    }
}
