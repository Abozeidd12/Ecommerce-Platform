using eCommerceCore.DTOs;
using eCommerceCore.IServices;
using eCommerceDomain.Entities;
using eCommerceDomain.IRepositeries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceCore.Services
{
    public class ProductService : IProductService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<GetProduct> AddProductAsync(AddProduct addProduct)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();




                // Create Product (inherits from User, so no separate User creation needed)
                var Product = new Product
                {
                    Name = addProduct.Name!,
                    Price = addProduct.Price,
                    CategoryId = addProduct.CategoryId


                };

                Product product = await _unitOfWork.Repository<Product>().AddAsync(Product);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Product created successfully ");

                // Return the newly created Product with all related data
                return await GetProductByIDAsync(Product.Id)
                    ?? throw new InvalidOperationException("Failed to retrieve created Product");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating Product");
                throw;
            }
        }

        public async Task<GetProduct?> GetProductByIDAsync(int id)
        {
            try
            {
                // Use advanced includes with ThenInclude
                var ProductEntit = await _unitOfWork.Repository<Product>()
                    .GetWithAdvancedIncludesAsync(c => c.Id == id, q => q.Include(p => p.Category));

                var ProductEntity = ProductEntit.FirstOrDefault();

                if (ProductEntity == null)
                    return null;

                return MapToGetProduct(ProductEntity, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Product");
                throw;
            }

        }

        public async Task<IEnumerable<GetProduct>> GetAllProductsAsync()
        {
            try
            {
                // Get all Products with includes
                var ProductEntities = await _unitOfWork.Repository<Product>()
                   .GetAllAsync();

                return ProductEntities
                    .Select(d => MapToGetProduct(d))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all Categories");
                throw;
            }
        }

        public async Task<(IEnumerable<GetProduct> Disabilities, int totalCount)> GetPagedProductsAsync(
            int pageNumber, int pageSize)
        {
            try
            {
                // First get the paged Product IDs
                var (Categories, totalCount) = await _unitOfWork.Repository<Product>()
                    .GetPagedAsync(
                        pageNumber,
                        pageSize

                    );

                var ssns = Categories.Select(p => p.Id).ToList();

                // Then load full data for those Products
                var fullProducts = await _unitOfWork.Repository<Product>()
                    .GetWithAdvancedIncludesAsync(p => ssns.Contains(p.Id), q => q.Include(p => p.Category));

                var result = fullProducts
                    .Select(d => MapToGetProduct(d,true))
                    .ToList();

                return (result, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged Categories");
                throw;
            }
        }

        public async Task<GetProduct> UpdateProductAsync(UpdateProduct updateProduct)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var Product = await _unitOfWork.Repository<Product>()
                    .FirstOrDefaultAsync(p => p.Id == updateProduct.Id);

                if (Product == null)
                    throw new KeyNotFoundException($"Product with ID {updateProduct.Id} not found");








                // Apply updates
                if (!string.IsNullOrWhiteSpace(updateProduct.Name))
                    Product.Name = updateProduct.Name;
                if (updateProduct.Price > 0)
                    Product.Price = updateProduct.Price;
                if (updateProduct.CategoryId != 0)
                    Product.CategoryId = updateProduct.CategoryId;
                



                await _unitOfWork.Repository<Product>().UpdateAsync(Product);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Product updated successfully ");

                var updatedProductDto = await GetProductByIDAsync(updateProduct.Id);

                if (updatedProductDto == null)
                {
                    throw new InvalidOperationException($"Failed to retrieve updated Product with ID: {updateProduct.Id}");
                }

                return updatedProductDto;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error updating Product with ID: {ID}", updateProduct.Id);
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var Product = await _unitOfWork.Repository<Product>()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (Product == null)
                    return false;

                await _unitOfWork.Repository<Product>().DeleteAsync(Product);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Product deleted successfully with ID: {id}", id);

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error deleting Product with ID: {ID}", id);
                throw;
            }
        }

        private GetProduct MapToGetProduct(Product ProductEntity, bool includeCollections = false)
        {
            var dto = new GetProduct
            {

                Name = ProductEntity.Name,
                Id = ProductEntity.Id,
                Price = ProductEntity.Price,



            };

            if (includeCollections)
            {

                dto.CategoryName = ProductEntity.Category.Name;
                dto.CategoryId = ProductEntity.CategoryId;
                

            }

            return dto;
        }



    }
}
