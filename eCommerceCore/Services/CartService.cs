using eCommerceCore.DTOs;
using eCommerceCore.IServices;
using eCommerceDomain.Entities;
using eCommerceDomain.IRepositeries;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceCore.Services
{
    public class CartService : ICartService
    {

          private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CartService> _logger;

        public CartService(IUnitOfWork unitOfWork, ILogger<CartService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<int> CreateCartAsync(string userId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();




                // Create Category (inherits from User, so no separate User creation needed)
                var existingCart = await _unitOfWork.Repository<Cart>().FirstOrDefaultAsync(c => c.UserId == userId);
                if (existingCart != null)
                {
                    _logger.LogInformation("Cart already Created ");

                    return existingCart.Id;
                }

                var cart = new Cart
                {
                    UserId = userId
                };

                Cart carttt = await _unitOfWork.Repository<Cart>().AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Cart created successfully ");

                // Return the newly created Category with all related data
                return carttt.Id;
                    
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating Cart");
                throw;
            }

        }

        public async Task DeleteCartAsync(string userId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var cart = await _unitOfWork.Repository<Cart>()
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                    return ;

                await _unitOfWork.Repository<Cart>().DeleteAsync(cart);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Cart deleted successfully with");

                
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error deleting Cart");
                throw;
            }
        }

        public async Task<IEnumerable<GetCartItem>> GetCartItemsAsync(string userId, int CartId)
        {
            var cart = await _unitOfWork.Repository<Cart>().FirstOrDefaultAsync(c => c.Id == CartId && c.UserId == userId);

            if(cart == null)
                throw new Exception("Cart is Empty or not found");



            var items = await _unitOfWork.Repository<CartItem>().GetAllIncludingAsync(c => c.Cart, c => c.Product);
           var itemss =  items.Where(c => c.CartId == CartId && c.Cart.UserId == userId).Select(c => new GetCartItem
            {
                cartId = c.CartId,
                productId = c.ProductId,
                ProductName = c.Product.Name,
                productPrice = c.Product.Price,
                quantity = c.Quantity,
                Id = c.Id
            }).ToList();

            if(itemss == null)
            {
                throw new Exception("Cart is Empty");
            }
            return itemss;
        }
    }
}
