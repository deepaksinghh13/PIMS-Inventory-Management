using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PIMS.Core.Dto.InventoryDto;
using PIMS.Application.IServices.IInventoryService;
using System.Threading.Tasks;
using PIMS.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PIMS.API.Controller
{
    /// <summary>
    /// Controller for managing inventory operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryAppService _inventoryService;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryController"/> class.
        /// </summary>
        /// <param name="inventoryService">The inventory service dependency.</param>
        public InventoryController(IInventoryAppService inventoryService, UserManager<User> userManager)
        {
            _inventoryService = inventoryService;
            _userManager = userManager;
        }

        /// <summary>
        /// Creates a new inventory item.
        /// </summary>
        [HttpPost("/adjust")]
        public async Task<IActionResult> CreateInventory([FromBody] CreateInventoryDto inventoryDto)
        {
            await _inventoryService.CreateInventoryAsync(inventoryDto);
            return NoContent();
        }


        /// <summary>
        /// Adjusts inventory quantity.
        /// </summary>
        /// <param name="inventoryId">The ID of the inventory item.</param>
        /// <param name="adjustmentDto">The adjustment details.</param>
        /// <returns>No content if successful.</returns>
        [HttpPost("{inventoryId}/adjustInventory")]
        public async Task<IActionResult> AdjustInventory(int inventoryId, [FromBody] AdjustInventoryDto adjustmentDto)
        {
            //var userIdClaim = User.FindFirstValue("nameid");
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("User ID is missing from the token.");
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest("Invalid user ID format.");
            }

            var userExists = await _userManager.Users.AnyAsync(u => u.Id == userId);

            await _inventoryService.AdjustInventoryAsync(inventoryId, adjustmentDto);
            return NoContent();
        }

        /// <summary>
        /// Retrieves inventory transactions.
        /// </summary>
        /// <param name="inventoryId">The ID of the inventory item.</param>
        /// <returns>A list of inventory transactions.</returns>
        [HttpGet("{inventoryId}/transactions")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetInventoryTransactions(int inventoryId)
        {
            var transactions = await _inventoryService.GetInventoryTransactionsAsync(inventoryId);
            return Ok(transactions);
        }

        /// <summary>
        /// Triggers low inventory alerts for items below the specified threshold.
        /// </summary>
        /// <param name="threshold">The threshold quantity for low inventory.</param>
        /// <returns>Message indicating completion of the low inventory check.</returns>
        [HttpGet("low-inventory-alert")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LowInventoryAlert([FromQuery] int threshold = 10)
        {
            await _inventoryService.CheckLowInventoryAsync(threshold);
            return Ok("Low inventory check completed. Alerts triggered if applicable.");
        }

        /// <summary>
        /// Gets inventory details by product ID.
        /// </summary>
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetInventoryByProductId(int productId)
        {
            var inventory = await _inventoryService.GetInventoryByProductIdAsync(productId);
            return Ok(inventory);
        }

        /// <summary>
        /// Manually adjusts inventory as part of an audit process.
        /// </summary>
        /// <param name="auditDto">The inventory audit details.</param>
        /// <returns>No content if successful.</returns>
        [HttpPost("audit-adjustment")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AuditAdjustment([FromBody] InventoryAuditDto auditDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _inventoryService.AdjustInventoryAsync(auditDto.InventoryId, auditDto.Adjustment);
            return NoContent();
        }
    }
}
