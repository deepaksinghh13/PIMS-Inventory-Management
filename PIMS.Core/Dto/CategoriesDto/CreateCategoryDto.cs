using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIMS.Core.Dto.CategoriesDto
{
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }
    }
}
