using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using PIMS.Application.Services.CategoryService;
using PIMS.Core;
using PIMS.Core.Dto.CategoriesDto;
using PIMS.Core.Models;
using PIMS.EntityFramework.EntityFramework;
using Xunit;

namespace PIMS.Tests.UnitTests
{
    public class CategoryAppServiceTests
    {
        private readonly CategoryAppService _service;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PrimsDbContext _dbContext;

        public CategoryAppServiceTests()
        {
            var options = new DbContextOptionsBuilder<PrimsDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new PrimsDbContext(options);
            _mockMapper = new Mock<IMapper>();
            _service = new CategoryAppService(_dbContext, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsMappedCategories()
        {
            // Arrange
            _dbContext.Categories.Add(new Category { CategoryID = 1, CategoryName = "Electronics" });
            _dbContext.SaveChanges();
            var categoryDtos = new List<CategoryDto> { new() { CategoryID = 1, CategoryName = "Electronics" } };
            _mockMapper.Setup(m => m.Map<List<CategoryDto>>(It.IsAny<List<Category>>())).Returns(categoryDtos);

            // Act
            var result = await _service.GetAllCategoriesAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Electronics", result[0].CategoryName);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ExistingCategory_ReturnsMappedCategory()
        {
            // Arrange
            var category = new Category { CategoryID = 2, CategoryName = "Books" };
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();
            _mockMapper.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>()))
                       .Returns(new CategoryDto { CategoryID = 2, CategoryName = "Books" });

            // Act
            var result = await _service.GetCategoryByIdAsync(2);

            // Assert
            Assert.Equal(2, result.CategoryID);
            Assert.Equal("Books", result.CategoryName);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_NonExistingCategory_ThrowsNotFoundException()
        {
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetCategoryByIdAsync(999));
        }

        [Fact]
        public async Task CreateCategoryAsync_ValidCategory_AddsCategory()
        {
            // Arrange
            var createDto = new CreateCategoryDto { CategoryName = "Furniture" };
            var category = new Category { CategoryID = 3, CategoryName = "Furniture" };
            _mockMapper.Setup(m => m.Map<Category>(It.IsAny<CreateCategoryDto>())).Returns(category);

            // Act
            await _service.CreateCategoryAsync(createDto);

            // Assert
            Assert.Equal(1, _dbContext.Categories.Count());
        }

        [Fact]
        public async Task UpdateCategoryAsync_NonExistingCategory_ThrowsNotFoundException()
        {
            var updateDto = new CreateCategoryDto { CategoryName = "Gaming" };
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateCategoryAsync(999, updateDto));
        }

        [Fact]
        public async Task DeleteCategoryAsync_ExistingCategory_DeletesCategory()
        {
            // Arrange
            var category = new Category { CategoryID = 5, CategoryName = "Music" };
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();

            // Act
            await _service.DeleteCategoryAsync(5);
            var deletedCategory = _dbContext.Categories.Find(5);

            // Assert
            Assert.Null(deletedCategory);
        }

        [Fact]
        public async Task DeleteCategoryAsync_NonExistingCategory_ThrowsNotFoundException()
        {
            await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteCategoryAsync(999));
        }
    }
}