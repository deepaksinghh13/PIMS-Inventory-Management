using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIMS.Core.Dto.ProductDto
{
    public class BulkPriceAdjustmentDto
    {
        public List<int> ProductIds { get; set; }

        [Range(-100, 100, ErrorMessage = "Percentage must be between -100% and 100%.")]
        public decimal? Percentage { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Fixed amount must be a positive value.")]
        public decimal? FixedAmount { get; set; }
    }
}
