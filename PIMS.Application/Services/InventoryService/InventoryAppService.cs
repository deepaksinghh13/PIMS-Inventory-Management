using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PIMS.Application.IServices.IInventoryService;
using PIMS.Core;
using PIMS.Core.Dto.InventoryDto;
using PIMS.Core.Models;
using PIMS.EntityFramework.EntityFramework;

namespace PIMS.Application.Services.InventoryService
{
    /// <summary>
    /// Service for managing inventory operations, including adjustments and transaction tracking.
    /// </summary>
    public class InventoryAppService : IInventoryAppService
    {
        private readonly PrimsDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryAppService"/> class.
        /// </summary>
        /// <param name="context">The database context for inventory management.</param>
        /// <param name="mapper">AutoMapper instance for DTO conversions.</param>
        public InventoryAppService(PrimsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new inventory item for a product.
        /// </summary>
        public async Task CreateInventoryAsync(CreateInventoryDto inventoryDto)
        {
            // Check if the product exists
            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .FirstOrDefaultAsync(p => p.ProductID == inventoryDto.ProductID);

            if (product == null)
                throw new NotFoundException("Product not found.");

            // Create the inventory
            var inventory = _mapper.Map<Inventory>(inventoryDto);
            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adjusts inventory quantity and logs the transaction.
        /// </summary>
        /// <param name="inventoryId">The ID of the inventory item to adjust.</param>
        /// <param name="adjustment">The DTO containing quantity change and reason.</param>
        /// <param name="userId">The ID of the user making the adjustment.</param>
        /// <exception cref="NotFoundException">Thrown when the inventory item is not found.</exception>
        /// <exception cref="ValidationException">Thrown when inventory quantity goes negative.</exception>
        public async Task AdjustInventoryAsync(int inventoryId, AdjustInventoryDto adjustment)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var inventory = await _context.Inventories.FindAsync(inventoryId);
                if (inventory == null)
                    throw new NotFoundException("Inventory not found.");

                // Update inventory quantity
                inventory.Quantity += adjustment.QuantityChange;
                if (inventory.Quantity < 0)
                    throw new ValidationException("Inventory quantity cannot be negative.");

                // Record transaction
                var transactionRecord = new InventoryTransaction
                {
                    InventoryID = inventoryId,
                    QuantityChange = adjustment.QuantityChange,
                    Reason = adjustment.Reason,
                    Timestamp = DateTime.UtcNow,
                };

                _context.InventoryTransactions.Add(transactionRecord);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Gets inventory details by product ID.
        /// </summary>
        public async Task<InventoriesDto> GetInventoryByProductIdAsync(int productId)
        {
            var inventory = await _context.Inventories
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.ProductID == productId);

            if (inventory == null)
                throw new NotFoundException("Inventory not found.");

            return _mapper.Map<InventoriesDto>(inventory);
        }

        /// <summary>
        /// Retrieves inventory transactions for a specific inventory item.
        /// </summary>
        /// <param name="inventoryId">The ID of the inventory item.</param>
        /// <returns>A list of inventory transaction DTOs.</returns>
        public async Task<List<InventoryTransactionDto>> GetInventoryTransactionsAsync(int inventoryId)
        {
            var transactions = await _context.InventoryTransactions
                .Where(it => it.InventoryID == inventoryId)
                .ToListAsync();

            return _mapper.Map<List<InventoryTransactionDto>>(transactions);
        }

        /// <summary>
        /// Checks for low inventory items below a specified threshold and triggers alerts.
        /// </summary>
        /// <param name="threshold">The inventory quantity threshold.</param>
        public async Task CheckLowInventoryAsync(int threshold)
        {
            var lowInventoryItems = await _context.Inventories
                .Where(i => i.Quantity < threshold)
                .Include(i => i.Product)
                .ToListAsync();

            foreach (var item in lowInventoryItems)
            {
                // Trigger alert (e.g., send email, log, or notify via API)
                Console.WriteLine($"Low inventory alert: Product {item.Product.Name} (ID: {item.ProductID}) has {item.Quantity} units left.");
            }
        }
    }
}
