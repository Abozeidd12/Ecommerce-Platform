using eCommerceCore.DTOs;
using eCommerceCore.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        private readonly ICategoryService _CategoryService;

        public CategoryController(ICategoryService CategoryService)
        {
            _CategoryService = CategoryService;
        }

        
        [HttpPost("AddCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GetCategory>> AddCategory([FromBody] AddCategory dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _CategoryService.AddCategoryAsync(dto);
                return CreatedAtAction(nameof(GetCategory), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while creating the Category" });
            }
        }

        [HttpGet("GetCategoryByID/{Id:int}")]
        public async Task<ActionResult<GetCategory>> GetCategory(int Id)
        {
            var Category = await _CategoryService.GetCategoryByIDAsync(Id);
            if (Category == null)
                return NotFound(new { message = $"Category with ID {Id} not found" });

            return Ok(Category);
        }

        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<IEnumerable<GetCategory>>> GetAllDisabilities()
        {
            var Categorys = await _CategoryService.GetAllCategoriesAsync();
            return Ok(Categorys);
        }

        [HttpPut("UpdateCategory")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<GetCategory>> UpdateCategory(
             [FromBody] UpdateCategory dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _CategoryService.UpdateCategoryAsync(dto);
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
                return StatusCode(500, new { message = "An error occurred while updating the Category" });
            }
        }

        [HttpDelete("DeleteCategory/{id:int}")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> DeleteCategory(int id)
        {
            var result = await _CategoryService.DeleteCategoryAsync(id);
            if (!result)
                return NotFound(new { message = $"Category with SSN {id} not found" });

            return NoContent();
        }

        [HttpGet("GetPagedCategories")]
        public async Task<ActionResult> GetPagedCategorys(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest(new { message = "Page number and page size must be greater than 0" });

            var (Categories, totalCount) = await _CategoryService.GetPagedCategoriesAsync(
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
