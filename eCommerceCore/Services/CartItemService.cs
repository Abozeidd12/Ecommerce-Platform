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
    public class CartItemService : ICartItemService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CartItemService> _logger;

        public CartItemService(IUnitOfWork unitOfWork, ILogger<CartItemService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task AddItemAsync(string userId, int cartId, AddCartItem dto)
        {

            try
            {
                await _unitOfWork.BeginTransactionAsync();




                // Create Category (inherits from User, so no separate User creation needed)
                var existingCart = await _unitOfWork.Repository<Cart>().FirstOrDefaultAsync(c => c.UserId == userId && c.Id == cartId);
                if (existingCart == null)
                {
                    _logger.LogInformation("Cart NOt Found");

                    return ;
                }

                var cartItem = await _unitOfWork.Repository<CartItem>().FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == dto.productId);

                if(cartItem != null)
                {
                    cartItem.Quantity += dto.quantity;

                }
                else
                {
                    var item = new CartItem
                    {
                        CartId = cartId,
                        ProductId = dto.productId,
                        Quantity = dto.quantity

                    };
                    CartItem carttt = await _unitOfWork.Repository<CartItem>().AddAsync(item);

                }

                
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("CartItem created successfully ");

                // Return the newly created Category with all related data
                

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating Cart");
                throw;
            }

        }

        public async Task<GetCartItem> GetItemAsync(string userId, int cartItemId)
        {
            var items = await _unitOfWork.Repository<CartItem>().GetAllIncludingAsync(c => c.Cart, c => c.Product);

            var item = items.FirstOrDefault(i => i.Id == cartItemId && i.Cart.UserId == userId);
            if (item == null)
                throw new Exception("Item not found");

            return new GetCartItem
            {
                cartId = item.CartId,
                productId = item.ProductId,
                ProductName = item.Product.Name,
                productPrice = item.Product.Price,
                quantity = item.Quantity,
                Id = item.Id

            };

            
            
        }

        public async Task UpdateItemAsync(string userId, int cartItemId, UpdateCartItem dto)
        {

            try
            {
                await _unitOfWork.BeginTransactionAsync();




                // Create Category (inherits from User, so no separate User creation needed)
                var items = await _unitOfWork.Repository<CartItem>().GetAllIncludingAsync(c => c.Cart, c => c.Product);

                var item = items.FirstOrDefault(i => i.Id == cartItemId && i.Cart.UserId == userId);
                if (item == null)
                    throw new Exception("Item not found");

                item.Quantity = dto.quantity;


                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("CartItem Updated successfully ");

                // Return the newly created Category with all related data


            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating Cart");
                throw;
            }
        }
    }
}
