using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIMS.Core.Dto.InventoryDto;

namespace PIMS.Application.IServices.IInventoryService
{
    public interface IInventoryAppService
    {
        Task CreateInventoryAsync(CreateInventoryDto inventoryDto);
        Task AdjustInventoryAsync(int inventoryId, AdjustInventoryDto adjustment);
        Task<List<InventoryTransactionDto>> GetInventoryTransactionsAsync(int inventoryId);
        Task CheckLowInventoryAsync(int threshold);
        Task<InventoriesDto> GetInventoryByProductIdAsync(int productId);
    }
}
