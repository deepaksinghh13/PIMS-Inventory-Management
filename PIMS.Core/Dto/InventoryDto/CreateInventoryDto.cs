using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIMS.Core.Dto.InventoryDto
{
    public class CreateInventoryDto
    {
        [Required]
        public int ProductID { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [StringLength(100)]
        public string WarehouseLocation { get; set; }
    }
}
