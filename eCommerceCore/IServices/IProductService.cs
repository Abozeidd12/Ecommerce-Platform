using eCommerceCore.DTOs;

namespace eCommerceCore.IServices
{
    public interface IProductService
    {

        Task<GetProduct> AddProductAsync(AddProduct addProduct);
        Task<GetProduct?> GetProductByIDAsync(int id);
        Task<IEnumerable<GetProduct>> GetAllProductsAsync();

        Task<GetProduct> UpdateProductAsync(UpdateProduct updateProduct);
        Task<bool> DeleteProductAsync(int id);
        Task<(IEnumerable<GetProduct> Disabilities, int totalCount)> GetPagedProductsAsync(int pageNumber, int pageSize);


    }
}
