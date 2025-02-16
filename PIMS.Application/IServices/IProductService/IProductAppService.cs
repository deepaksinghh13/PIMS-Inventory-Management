using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIMS.Core.Dto.ProductDto;

namespace PIMS.Application.IServices.IProductService
{
    public interface IProductAppService
    {
        Task<List<ProductsDto>> GetAllProductsAsync(int? categoryId = null);
        Task<ProductsDto> GetProductByIdAsync(int id);
        Task CreateProductAsync(CreateProductDto productDto);
        Task UpdateProductAsync(int id, UpdateProductDto productDto);
        Task DeleteProductAsync(int id);
        Task AdjustPriceAsync(int productId, decimal newPrice);
        Task AdjustBulkPricesAsync(List<int> productIds, decimal? percentage, decimal? fixedAmount);

    }
}
