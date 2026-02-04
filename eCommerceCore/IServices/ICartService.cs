using eCommerceCore.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceCore.IServices
{
    public interface ICartService
    {
        Task<int> CreateCartAsync(string userId);
        Task DeleteCartAsync(string userId);

        Task <IEnumerable<GetCartItem>> GetCartItemsAsync(string userId, int CartId);


    }
}
