using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIMS.Core.Dto.InventoryDto
{
    public class InventoryTransactionDto
    {
        public int TransactionID { get; set; }
        public int InventoryID { get; set; }
        public int QuantityChange { get; set; }
        public string Reason { get; set; }
        public DateTime Timestamp { get; set; }
        public int UserID { get; set; }
    }
}
