using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIMS.Core.Dto.InventoryDto
{
    public class AdjustInventoryDto
    {
        [Required]
        public int QuantityChange { get; set; }

        [Required]
        [StringLength(200)]
        public string Reason { get; set; }
    }
}
