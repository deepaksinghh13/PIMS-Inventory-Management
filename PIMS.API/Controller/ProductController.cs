using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIMS.Application.IServices.IProductService;
using PIMS.Core.Dto.ProductDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PIMS.API.Controllers
{
    /// <summary>
    /// Controller for managing products.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    //[Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductAppService _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductController"/> class.
        /// </summary>
        /// <param name="productService">Service for handling product operations.</param>
        public ProductController(IProductAppService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Retrieves all products, optionally filtered by category.
        /// </summary>
        /// <param name="categoryId">Optional category ID to filter products.</param>
        /// <returns>List of products.</returns>
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] int? categoryId)
        {
            var products = await _productService.GetAllProductsAsync(categoryId);
            return Ok(products);
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>The requested product.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return Ok(product);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="productDto">DTO containing product details.</param>
        /// <returns>No content.</returns>
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        {
            await _productService.CreateProductAsync(productDto);
            return NoContent();
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="productDto">DTO containing updated product details.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto productDto)
        {
            await _productService.UpdateProductAsync(id, productDto);
            return NoContent();
        }

        /// <summary>
        /// Deletes a product.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Adjusts the price of a specific product.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <param name="newPrice">The new price value.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id}/price")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdjustPrice(int id, [FromBody] decimal newPrice)
        {
            await _productService.AdjustPriceAsync(id, newPrice);
            return NoContent();
        }

        /// <summary>
        /// Performs a bulk price adjustment for multiple products.
        /// </summary>
        /// <param name="adjustmentDto">DTO containing product IDs and adjustment values.</param>
        /// <returns>No content.</returns>
        [HttpPost("bulk-price-adjustment")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkPriceAdjustment([FromBody] BulkPriceAdjustmentDto adjustmentDto)
        {
            await _productService.AdjustBulkPricesAsync(adjustmentDto.ProductIds, adjustmentDto.Percentage, adjustmentDto.FixedAmount);
            return NoContent();
        }
    }
}
