using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using PIMS.Application.Services.ProductServices;
using PIMS.Core;
using PIMS.Core.Dto.ProductDto;
using PIMS.Core.Models;
using PIMS.EntityFramework.EntityFramework;

namespace PIMS.Tests.UnitTests
{
    public class ProductAppServiceTests
    {
        private readonly ProductAppService _service;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PrimsDbContext _dbContext;

        public ProductAppServiceTests()
        {
            var options = new DbContextOptionsBuilder<PrimsDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new PrimsDbContext(options);
            _mockMapper = new Mock<IMapper>();
            _service = new ProductAppService(_dbContext, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsAllProducts()
        {
            // Arrange
            _dbContext.Products.Add(new Product { ProductID = 1, Name = "Laptop", Description = "A powerful laptop", SKU = "LAP123", Price = 1000 });
            _dbContext.SaveChanges();
            var productsDto = new List<ProductsDto> { new() { ProductID = 1, Name = "Laptop", SKU = "LAP123", Description = "A powerful laptop", Price = 1000 } };
            _mockMapper.Setup(m => m.Map<List<ProductsDto>>(It.IsAny<List<Product>>())).Returns(productsDto);

            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Laptop", result[0].Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_ExistingProduct_ReturnsMappedProduct()
        {
            // Arrange
            var product = new Product { ProductID = 2, Name = "Tablet", SKU = "TAB456", Description = "A powerful laptop", Price = 500 };
            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
            _mockMapper.Setup(m => m.Map<ProductsDto>(It.IsAny<Product>()))
                       .Returns(new ProductsDto { ProductID = 2, Name = "Tablet", SKU = "TAB456", Description = "A powerful laptop", Price = 500 });

            // Act
            var result = await _service.GetProductByIdAsync(2);

            // Assert
            Assert.Equal(2, result.ProductID);
            Assert.Equal("Tablet", result.Name);
        }

        [Fact]
        public async Task CreateProductAsync_DuplicateSKU_ThrowsValidationException()
        {
            // Arrange
            var existingProduct = new Product { ProductID = 4, Name = "Monitor", Description = "A powerful laptop", SKU = "MON111", Price = 150 };
            _dbContext.Products.Add(existingProduct);
            _dbContext.SaveChanges();

            var createDto = new CreateProductDto { Name = "Monitor", SKU = "MON111", Price = 200, CategoryIds = new List<int>() };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.CreateProductAsync(createDto));
        }

        [Fact]
        public async Task DeleteProductAsync_ExistingProduct_DeletesProduct()
        {
            // Arrange
            var product = new Product { ProductID = 5, Name = "Mouse", Description = "A powerful laptop", SKU = "MSE456", Price = 30 };
            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();

            // Act
            await _service.DeleteProductAsync(5);
            var deletedProduct = _dbContext.Products.Find(5);

            // Assert
            Assert.Null(deletedProduct);
        }

        [Fact]
        public async Task AdjustPriceAsync_NegativePrice_ThrowsValidationException()
        {
            await Assert.ThrowsAsync<ValidationException>(() => _service.AdjustPriceAsync(1, -10));
        }

        [Fact]
        public async Task AdjustBulkPricesAsync_NoValidProductIds_ThrowsNotFoundException()
        {
            await Assert.ThrowsAsync<NotFoundException>(() => _service.AdjustBulkPricesAsync(new List<int> { 999 }, 10, null));
        }
    }
}
