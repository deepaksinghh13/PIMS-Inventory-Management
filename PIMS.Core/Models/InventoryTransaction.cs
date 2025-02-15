using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIMS.Core.Models
{
    public class InventoryTransaction
    {
        [Key]
        public int TransactionID { get; set; }

        [Required]
        public int InventoryID { get; set; }
        public Inventory Inventory { get; set; }

        [Required]
        public int QuantityChange { get; set; }

        [Required]
        [StringLength(200)]
        public string Reason { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required]
        public int UserID { get; set; }
        public User User { get; set; }
    }
}
