using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Agullto_IMS.Models;
using Agullto_IMS.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Agullto_IMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryManagementController : ControllerBase
    {
        private readonly ProductService _productService;

        public InventoryManagementController(ProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        // 1. GET ALL PRODUCTS -> GET: api/inventorymanagement
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetAllProducts()
        {
            try
            {
                var products = _productService.GetAllProducts();
                if (products == null)
                {
                    return NotFound(new { message = "Product repository data is unavailable." });
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllProducts: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while retrieving products. Please try again later.",
                    error = ex.Message
                });
            }
        }

        // 2. GET PRODUCT BY ID -> GET: api/inventorymanagement/{id}
        [HttpGet("{id}")]
        public ActionResult<Product> GetProductById(Guid id)
        {
            // Input Validation
            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "The provided Product ID is invalid or empty." });
            }

            try
            {
                var product = _productService.FindProduct(id);
                if (product == null)
                {
                    return NotFound(new { message = $"Product with ID {id} was not found." });
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProductById for ID {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while retrieving the product detail.",
                    error = ex.Message
                });
            }
        }

        // 3. GET LOW STOCK ITEMS -> GET: api/inventorymanagement/low-stock
        [HttpGet("low-stock")]
        public ActionResult<List<Product>> GetLowStockItems()
        {
            try
            {
                var lowStockProducts = _productService.GetAllProducts();

                if (lowStockProducts == null)
                {
                    return NotFound(new { message = "Could not retrieve products. Product list is unavailable." });
                }

                var filteredProducts = lowStockProducts
                    .Where(p => p.Stock < 5)
                    .ToList();

                return Ok(filteredProducts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetLowStockItems: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while retrieving low stock items.",
                    error = ex.Message
                });
            }
        }

        // 4. CREATE PRODUCT -> POST: api/inventorymanagement
        [HttpPost]
        public ActionResult<Product> CreateProduct([FromBody] Product product)
        {
            // Input Validation
            if (product == null)
            {
                return BadRequest(new { message = "Product data payload cannot be null." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _productService.AddProduct(product);
                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateProduct: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while saving the new product.",
                    error = ex.Message
                });
            }
        }

        // 5. UPDATE PRODUCT -> PUT: api/inventorymanagement
        [HttpPut]
        public IActionResult UpdateProduct([FromBody] Product product)
        {
            // Input Validation
            if (product == null)
            {
                return BadRequest(new { message = "Product update data payload cannot be null." });
            }

            if (product.Id == Guid.Empty)
            {
                return BadRequest(new { message = "A valid Product ID must be provided within the data body for updating." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                bool wasUpdated = _productService.UpdateProduct(product);
                if (!wasUpdated)
                {
                    return NotFound(new { message = $"Failed to update. Product with ID {product.Id} not found." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateProduct for ID {product.Id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while updating the product entry.",
                    error = ex.Message
                });
            }
        }

        // 6. DELETE PRODUCT -> DELETE: api/inventorymanagement/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(Guid id)
        {
            // Input Validation
            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "The provided Product ID is invalid or empty." });
            }

            try
            {
                bool wasDeleted = _productService.DeleteProduct(id);
                if (!wasDeleted)
                {
                    return NotFound(new { message = $"Failed to delete. Product with ID {id} not found." });
                }

                return Ok(new { message = "Product deleted successfully from the inventory system." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteProduct for ID {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while executing the product deletion process.",
                    error = ex.Message
                });
            }
        }
    }
}