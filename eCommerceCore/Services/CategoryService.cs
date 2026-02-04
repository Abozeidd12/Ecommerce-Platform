using eCommerceCore.DTOs;
using eCommerceCore.IServices;
using eCommerceDomain.Entities;
using eCommerceDomain.IRepositeries;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceCore.Services
{
    public class CategoryService : ICategoryService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<GetCategory> AddCategoryAsync(AddCategory addCategory)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

              


                // Create Category (inherits from User, so no separate User creation needed)
                var Category = new Category
                {
                    Name = addCategory.Name!

                    
                };

              Category category =   await _unitOfWork.Repository<Category>().AddAsync(Category);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Category created successfully ");

                // Return the newly created Category with all related data
                return await GetCategoryByIDAsync(category.Id)
                    ?? throw new InvalidOperationException("Failed to retrieve created Category");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating Category");
                throw;
            }
        }

        public async Task<GetCategory?> GetCategoryByIDAsync(int id)
        {
            try
            {
                // Use advanced includes with ThenInclude
                var CategoryEntit = await _unitOfWork.Repository<Category>()
                    .GetWithAdvancedIncludesAsync(c => c.Id == id, q => q.Include(c => c.Products));

                var CategoryEntity = CategoryEntit.FirstOrDefault();

                if (CategoryEntity == null)
                    return null;

                return MapToGetCategory(CategoryEntity, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Category");
                throw;
            }

        }

        public async Task<IEnumerable<GetCategory>> GetAllCategoriesAsync()
        {
            try
            {
                // Get all Categorys with includes
                var CategoryEntities = await _unitOfWork.Repository<Category>()
                   .GetAllAsync();

                return CategoryEntities
                    .Select(d => MapToGetCategory(d))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all Categories");
                throw;
            }
        }

        public async Task<(IEnumerable<GetCategory> Disabilities, int totalCount)> GetPagedCategoriesAsync(
            int pageNumber, int pageSize)
        {
            try
            {
                // First get the paged Category IDs
                var (Categories, totalCount) = await _unitOfWork.Repository<Category>()
                    .GetPagedAsync(
                        pageNumber,
                        pageSize

                    );

                var ssns = Categories.Select(p => p.Id).ToList();

                // Then load full data for those Categorys
                var fullCategorys = await _unitOfWork.Repository<Category>()
                    .GetWithAdvancedIncludesAsync(p => ssns.Contains(p.Id));

                var result = fullCategorys
                    .Select(d => MapToGetCategory(d))
                    .ToList();

                return (result, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged Categories");
                throw;
            }
        }

        public async Task<GetCategory> UpdateCategoryAsync(UpdateCategory updateCategory)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var Category = await _unitOfWork.Repository<Category>()
                    .FirstOrDefaultAsync(p => p.Id == updateCategory.Id);

                if (Category == null)
                    throw new KeyNotFoundException($"Category with ID {updateCategory.Id} not found");








                // Apply updates
                if (!string.IsNullOrWhiteSpace(updateCategory.Name))
                    Category.Name = updateCategory.Name;
                


                await _unitOfWork.Repository<Category>().UpdateAsync(Category);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Category updated successfully ");

                var updatedCategoryDto = await GetCategoryByIDAsync(updateCategory.Id);

                if (updatedCategoryDto == null)
                {
                    throw new InvalidOperationException($"Failed to retrieve updated Category with ID: {updateCategory.Id}");
                }

                return updatedCategoryDto;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error updating Category with ID: {ID}", updateCategory.Id);
                throw;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var Category = await _unitOfWork.Repository<Category>()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (Category == null)
                    return false;

                await _unitOfWork.Repository<Category>().DeleteAsync(Category);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Category deleted successfully with ID: {id}", id);

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error deleting Category with ID: {ID}", id);
                throw;
            }
        }

        private GetCategory MapToGetCategory(Category CategoryEntity, bool includeCollections = false)
        {
            var dto = new GetCategory
            {

                Name = CategoryEntity.Name,
                Id = CategoryEntity.Id

                

            };

            if (includeCollections)
            {

                dto.getProducts = CategoryEntity.Products.Select(p => new GetProduct
                {
                    Name = p.Name,
                    Price = p.Price,
                    Id = p.Id,
                    CategoryName = CategoryEntity.Name,
                    CategoryId = CategoryEntity.Id
                }).ToList();

            }

            return dto;
        }



    }
}
