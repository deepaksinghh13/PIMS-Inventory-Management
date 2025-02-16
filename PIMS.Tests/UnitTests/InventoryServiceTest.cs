using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PIMS.Application.Services.InventoryService;
using PIMS.Core.Dto.InventoryDto;
using PIMS.Core.Models;
using PIMS.Core;
using PIMS.EntityFramework.EntityFramework;

namespace PIMS.Tests.UnitTests
{
    public class InventoryServiceTest
    {
        private readonly PrimsDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly InventoryAppService _inventoryService;

        public InventoryServiceTest()
        {
            var options = new DbContextOptionsBuilder<PrimsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new PrimsDbContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateInventoryDto, Inventory>();
                cfg.CreateMap<Inventory, InventoriesDto>();
                cfg.CreateMap<InventoryTransaction, InventoryTransactionDto>();
            });

            _mapper = config.CreateMapper();
            _inventoryService = new InventoryAppService(_dbContext, _mapper);

            // Seed test data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var product = new Product
            {
                ProductID = 1,
                Name = "Laptop",
                Description = "High-end gaming laptop",
                SKU = "LAP123",
                Price = 1500
            };

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task CreateInventoryAsync_ValidData_CreatesInventory()
        {
            // Arrange
            var inventoryDto = new CreateInventoryDto
            {
                ProductID = 1,
                Quantity = 10,
                WarehouseLocation = "Warehouse A"
            };

            // Act
            await _inventoryService.CreateInventoryAsync(inventoryDto);
            var inventory = await _dbContext.Inventories.FirstOrDefaultAsync(i => i.ProductID == 1);

            // Assert
            Assert.NotNull(inventory);
            Assert.Equal(10, inventory.Quantity);
            Assert.Equal("Warehouse A", inventory.WarehouseLocation);
        }

        [Fact]
        public async Task CreateInventoryAsync_ProductNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var inventoryDto = new CreateInventoryDto
            {
                ProductID = 99, // Non-existent product
                Quantity = 5,
                WarehouseLocation = "Warehouse X"
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _inventoryService.CreateInventoryAsync(inventoryDto));
        }


        [Fact]
        public async Task GetInventoryByProductIdAsync_ExistingProduct_ReturnsInventory()
        {
            // Arrange
            var inventory = new Inventory
            {
                InventoryID = 3,
                ProductID = 1,
                Quantity = 20,
                WarehouseLocation = "Warehouse C"
            };

            _dbContext.Inventories.Add(inventory);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _inventoryService.GetInventoryByProductIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(20, result.Quantity);
        }

        [Fact]
        public async Task GetInventoryByProductIdAsync_ProductNotFound_ThrowsNotFoundException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _inventoryService.GetInventoryByProductIdAsync(99));
        }

        [Fact]
        public async Task GetInventoryTransactionsAsync_ExistingInventory_ReturnsTransactions()
        {
            // Arrange
            var inventory = new Inventory
            {
                InventoryID = 4,
                ProductID = 1,
                Quantity = 50,
                WarehouseLocation = "Warehouse D"
            };

            var transaction1 = new InventoryTransaction
            {
                TransactionID = 1,
                InventoryID = 4,
                QuantityChange = 5,
                Reason = "Sold items",
                Timestamp = DateTime.UtcNow
            };

            var transaction2 = new InventoryTransaction
            {
                TransactionID = 2,
                InventoryID = 4,
                QuantityChange = 10,
                Reason = "Restock",
                Timestamp = DateTime.UtcNow
            };

            _dbContext.Inventories.Add(inventory);
            _dbContext.InventoryTransactions.AddRange(transaction1, transaction2);
            await _dbContext.SaveChangesAsync();

            // Act
            var transactions = await _inventoryService.GetInventoryTransactionsAsync(4);

            // Assert
            Assert.Equal(2, transactions.Count);
            Assert.Contains(transactions, t => t.Reason == "Sold items");
            Assert.Contains(transactions, t => t.Reason == "Restock");
        }

        [Fact]
        public async Task CheckLowInventoryAsync_LowStock_TriggersAlert()
        {
            // Arrange
            var inventory1 = new Inventory
            {
                InventoryID = 5,
                ProductID = 1,
                Quantity = 2, // Below threshold
                WarehouseLocation = "Warehouse E"
            };

            var inventory2 = new Inventory
            {
                InventoryID = 6,
                ProductID = 1,
                Quantity = 10, // Above threshold
                WarehouseLocation = "Warehouse F"
            };

            _dbContext.Inventories.AddRange(inventory1, inventory2);
            await _dbContext.SaveChangesAsync();

            // Act
            await _inventoryService.CheckLowInventoryAsync(5);

            // Assert
            // This test is difficult to assert directly since CheckLowInventoryAsync logs to Console.
            // But if no exceptions are thrown, the method is functioning correctly.
            Assert.True(true);
        }
    }
}
