using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIMS.Core.Dto.InventoryDto
{
    public class InventoryAuditDto
    {
        public int InventoryId { get; set; }
        public AdjustInventoryDto Adjustment { get; set; }
    }
}
