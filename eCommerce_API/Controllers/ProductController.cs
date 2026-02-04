using eCommerceCore.DTOs;
using eCommerceCore.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _ProductService;

        public ProductController(IProductService ProductService)
        {
            _ProductService = ProductService;
        }

        [HttpPost("AddProduct")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<GetProduct>> AddProduct([FromBody] AddProduct dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _ProductService.AddProductAsync(dto);
                return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while creating the Product" });
            }
        }

        [HttpGet("GetProductByID/{Id:int}")]
        public async Task<ActionResult<GetProduct>> GetProduct(int Id)
        {
            var Product = await _ProductService.GetProductByIDAsync(Id);
            if (Product == null)
                return NotFound(new { message = $"Product with ID {Id} not found" });

            return Ok(Product);
        }

        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<IEnumerable<GetProduct>>> GetAllDisabilities()
        {
            var Products = await _ProductService.GetAllProductsAsync();
            return Ok(Products);
        }

        [HttpPut("UpdateProduct")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<GetProduct>> UpdateProduct(
             [FromBody] UpdateProduct dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _ProductService.UpdateProductAsync(dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while updating the Product" });
            }
        }

        [HttpDelete("DeleteProduct/{id:int}")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> DeleteProduct(int id)
        {
            var result = await _ProductService.DeleteProductAsync(id);
            if (!result)
                return NotFound(new { message = $"Product with SSN {id} not found" });

            return NoContent();
        }

        [HttpGet("GetPagedProducts")]
        public async Task<ActionResult> GetPagedProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest(new { message = "Page number and page size must be greater than 0" });

            var (Categories, totalCount) = await _ProductService.GetPagedProductsAsync(
                pageNumber, pageSize);

            return Ok(new
            {
                data = Categories,
                pagination = new
                {
                    totalCount,
                    pageNumber,
                    pageSize,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            });
        }
    }
}
