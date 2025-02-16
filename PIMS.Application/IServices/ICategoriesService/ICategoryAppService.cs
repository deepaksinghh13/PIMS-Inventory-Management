using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIMS.Core.Dto.CategoriesDto;

namespace PIMS.Application.IServices.ICategoriesService
{
    public interface ICategoryAppService
    {
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task CreateCategoryAsync(CreateCategoryDto categoryDto);
        Task UpdateCategoryAsync(int id, CreateCategoryDto categoryDto);
        Task DeleteCategoryAsync(int id);
    }
}
