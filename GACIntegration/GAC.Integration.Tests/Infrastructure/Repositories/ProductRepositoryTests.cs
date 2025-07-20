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
    public class ProductRepositoryTests
    {
        private static AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Product_To_Database()
        {
            var context = CreateDbContext();
            var repository = new ProductRepository(context);

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Code = "PRD001",
                Title = "Test Product",
                Description = "A sample product",
                Dimensions = "10x10x5"
            };

            await repository.AddAsync(product);

            var products = await context.Products.ToListAsync();
            products.Should().ContainSingle();
            products[0].Title.Should().Be("Test Product");
            products[0].Code.Should().Be("PRD001");
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Products()
        {
            var context = CreateDbContext();
            context.Products.AddRange(
                new Product
                {
                    Id = Guid.NewGuid(),
                    Code = "PRD001",
                    Title = "Product A",
                    Description = "Desc A",
                    Dimensions = "10x5x2"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Code = "PRD002",
                    Title = "Product B",
                    Description = "Desc B",
                    Dimensions = "15x5x3"
                }
            );
            await context.SaveChangesAsync();

            var repository = new ProductRepository(context);

            var result = await repository.GetAllAsync();

            result.Should().HaveCount(2);
            result.Should().Contain(p => p.Title == "Product A");
            result.Should().Contain(p => p.Code == "PRD002");
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_Product()
        {
            var context = CreateDbContext();
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Code = "P123",
                Title = "Test Product",
                Description = "Sample",
                Dimensions = "5x5"
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var repository = new ProductRepository(context);

            var result = await repository.GetByIdAsync(product.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(product.Id);
            result.Title.Should().Be("Test Product");
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Product_Details()
        {
            var context = CreateDbContext();
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Code = "P456",
                Title = "Old Title",
                Description = "Old Desc",
                Dimensions = "1x2"
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var repository = new ProductRepository(context);

            product.Title = "Updated Title";
            product.Description = "Updated Desc";
            product.Dimensions = "3x4";

            await repository.UpdateAsync(product);

            var updated = await context.Products.FindAsync(product.Id);
            updated!.Title.Should().Be("Updated Title");
            updated.Description.Should().Be("Updated Desc");
            updated.Dimensions.Should().Be("3x4");
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Product_From_Database()
        {
            var context = CreateDbContext();
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Code = "P999",
                Title = "To Delete",
                Description = "Delete me",
                Dimensions = "1x1"
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var repository = new ProductRepository(context);

            await repository.DeleteAsync(product.Id);

            var result = await context.Products.FindAsync(product.Id);
            result.Should().BeNull();
        }
    }
}
