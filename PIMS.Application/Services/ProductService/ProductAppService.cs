using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PIMS.Application.IServices.IProductService;
using PIMS.Core;
using PIMS.Core.Dto.ProductDto;
using PIMS.Core.Models;
using PIMS.EntityFramework.EntityFramework;

namespace PIMS.Application.Services.ProductServices
{
    /// <summary>
    /// Service class responsible for handling product-related business logic.
    /// </summary>
    public class ProductAppService : IProductAppService
    {
        private readonly PrimsDbContext _context;
        private readonly IMapper _mapper;

        public ProductAppService(PrimsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of products, optionally filtered by category.
        /// </summary>
        /// <param name="categoryId">Optional category ID to filter products.</param>
        /// <returns>List of products as DTOs.</returns>
        public async Task<List<ProductsDto>> GetAllProductsAsync(int? categoryId = null)
        {
            var query = _context.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .AsQueryable();

            if (categoryId != null)
                query = query.Where(p => p.ProductCategories.Any(pc => pc.CategoryID == categoryId));

            var products = await query.ToListAsync();
            return _mapper.Map<List<ProductsDto>>(products);
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>The product DTO.</returns>
        public async Task<ProductsDto> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            return _mapper.Map<ProductsDto>(product);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="productDto">DTO containing product details.</param>
        public async Task CreateProductAsync(CreateProductDto productDto)
        {
            if (await _context.Products.AnyAsync(p => p.SKU == productDto.SKU))
                throw new ValidationException("SKU must be unique.");

            var product = _mapper.Map<Product>(productDto);

            foreach (var categoryId in productDto.CategoryIds)
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category != null)
                    product.ProductCategories.Add(new ProductCategory { Category = category });
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <param name="productDto">DTO containing updated product details.</param>
        public async Task UpdateProductAsync(int id, UpdateProductDto productDto)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
                throw new NotFoundException("Product not found.");

            _mapper.Map(productDto, product);

            product.ProductCategories.Clear();
            foreach (var categoryId in productDto.CategoryIds)
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category != null)
                    product.ProductCategories.Add(new ProductCategory { Category = category });
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id">The product ID.</param>
        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new NotFoundException("Product not found.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adjusts the price of a specific product.
        /// </summary>
        /// <param name="productId">The product ID.</param>
        /// <param name="newPrice">The new price for the product.</param>
        public async Task AdjustPriceAsync(int productId, decimal newPrice)
        {
            if (newPrice < 0)
                throw new ValidationException("Price cannot be negative.");

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new NotFoundException("Product not found.");

            product.Price = newPrice;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adjusts prices for multiple products either by a percentage or a fixed amount.
        /// </summary>
        /// <param name="productIds">List of product IDs to adjust.</param>
        /// <param name="percentage">Optional percentage to adjust prices by.</param>
        /// <param name="fixedAmount">Optional fixed amount to adjust prices by.</param>
        public async Task AdjustBulkPricesAsync(List<int> productIds, decimal? percentage, decimal? fixedAmount)
        {
            if (percentage == null && fixedAmount == null)
                throw new ValidationException("Either percentage or fixed amount must be provided.");

            if (percentage != null && fixedAmount != null)
                throw new ValidationException("Cannot use both percentage and fixed amount adjustments.");

            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductID))
                .ToListAsync();

            if (products.Count == 0)
                throw new NotFoundException("No products found for the provided IDs.");

            foreach (var product in products)
            {
                if (percentage != null)
                {
                    var percentageValue = (decimal)percentage / 100;
                    product.Price += product.Price * percentageValue;
                }
                else if (fixedAmount != null)
                {
                    product.Price += (decimal)fixedAmount;
                }

                if (product.Price < 0)
                    throw new ValidationException($"Price for product {product.ProductID} cannot be negative.");
            }

            await _context.SaveChangesAsync();
        }
    }
}
