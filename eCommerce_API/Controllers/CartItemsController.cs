using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce_API.Controllers
{
    using eCommerce_API.Extensions;
    using eCommerceCore.DTOs;
    using eCommerceCore.IServices;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    namespace eCommerce.API.Controllers
    {
        [Authorize]
        [ApiController]
        [Route("api/[controller]")]
        public class CartItemsController : ControllerBase
        {
            private readonly ICartItemService _cartItemService;

            public CartItemsController(ICartItemService cartItemService)
            {
                _cartItemService = cartItemService;
            }

            [HttpPost("{cartId:int}")]
            public async Task<IActionResult> AddCartItem(
                int cartId,
                [FromBody] AddCartItem dto)
            {
                var userId = User.GetUserId();

                await _cartItemService.AddItemAsync(userId, cartId, dto);

                return Ok(new { message = "Cart item added successfully" });
            }

            [HttpGet("{cartItemId:int}")]
            public async Task<IActionResult> GetCartItem(int cartItemId)
            {
                var userId = User.GetUserId();

                var item = await _cartItemService.GetItemAsync(userId, cartItemId);

                return Ok(item);
            }

            [HttpPatch("{cartItemId:int}")]
            public async Task<IActionResult> UpdateCartItem(
                int cartItemId,
                [FromBody] UpdateCartItem dto)
            {
                var userId = User.GetUserId();

                await _cartItemService.UpdateItemAsync(userId, cartItemId, dto);

                return Ok(new { message = "Cart item updated successfully" });
            }
        }
    }

}
