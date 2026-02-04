using eCommerce_API.Extensions;
using eCommerceCore.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eCommerce_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(
            ICartService cartService,
            ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCart()
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Creating cart for user {UserId}", userId);

            var cartId = await _cartService.CreateCartAsync(userId);

            _logger.LogInformation("Cart {CartId} created for user {UserId}", cartId, userId);
            return Ok(new { cartId });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCart()
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Deleting cart for user {UserId}", userId);

            await _cartService.DeleteCartAsync(userId);

            _logger.LogInformation("Cart deleted for user {UserId}", userId);
            return NoContent();
        }

        [HttpGet("{cartId:int}")]
        public async Task<IActionResult> GetCartItems(int cartId)
        {
            var userId = User.GetUserId();
            _logger.LogInformation(
                "Getting cart items for cart {CartId} and user {UserId}",
                cartId, userId);

            var items = await _cartService.GetCartItemsAsync(userId, cartId);

            return Ok(items);
        }
    }
}
