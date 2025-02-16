using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIMS.Core.Dto.InventoryDto
{
    public class InventoriesDto
    {
        public int InventoryID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public string WarehouseLocation { get; set; }
    }
}
