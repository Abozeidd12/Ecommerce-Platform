using eCommerceCore.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceCore.IServices
{
    public interface ICartItemService
    {
        Task AddItemAsync(string userId, int cartId, AddCartItem dto);
        Task<GetCartItem> GetItemAsync(string userId, int cartItemId);
        Task UpdateItemAsync(string userId, int cartItemId, UpdateCartItem dto);
    }
}
