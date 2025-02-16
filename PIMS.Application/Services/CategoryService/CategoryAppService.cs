using AutoMapper;
using PIMS.Application.IServices.ICategoriesService;
using PIMS.Core.Dto.CategoriesDto;
using PIMS.Core.Models;
using PIMS.Core;
using PIMS.EntityFramework.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace PIMS.Application.Services.CategoryService
{
    /// <summary>
    /// Service class for managing product categories.
    /// </summary>
    public class CategoryAppService : ICategoryAppService
    {
        private readonly PrimsDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryAppService"/> class.
        /// </summary>
        /// <param name="context">Database context for category operations.</param>
        /// <param name="mapper">Mapper for object transformation.</param>
        public CategoryAppService(PrimsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        /// <returns>List of category DTOs.</returns>
        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        /// <summary>
        /// Retrieves a category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category.</param>
        /// <returns>The category DTO.</returns>
        /// <exception cref="NotFoundException">Thrown when category is not found.</exception>
        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                throw new NotFoundException("Category not found.");

            return _mapper.Map<CategoryDto>(category);
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="categoryDto">DTO containing category details.</param>
        public async Task CreateCategoryAsync(CreateCategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id">The ID of the category to update.</param>
        /// <param name="categoryDto">DTO containing updated category details.</param>
        /// <exception cref="NotFoundException">Thrown when category is not found.</exception>
        public async Task UpdateCategoryAsync(int id, CreateCategoryDto categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                throw new NotFoundException("Category not found.");

            _mapper.Map(categoryDto, category);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a category.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <exception cref="NotFoundException">Thrown when category is not found.</exception>
        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                throw new NotFoundException("Category not found.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
