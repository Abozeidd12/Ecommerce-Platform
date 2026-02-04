using eCommerceCore.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceCore.IServices
{
    public interface ICategoryService
    {

        Task<GetCategory> AddCategoryAsync(AddCategory addCategory);
        Task<GetCategory?> GetCategoryByIDAsync(int id);
        Task<IEnumerable<GetCategory>> GetAllCategoriesAsync();

        Task<GetCategory> UpdateCategoryAsync(UpdateCategory updateCategory);
        Task<bool> DeleteCategoryAsync(int id);
        Task<(IEnumerable<GetCategory> Disabilities, int totalCount)> GetPagedCategoriesAsync(int pageNumber, int pageSize);


    }
}
