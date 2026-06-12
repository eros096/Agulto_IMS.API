using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Agullto_IMS.Models;          // Core library models reference
using Agullto_IMS.Services;        // Core services reference
using Agullto_IMS.API.Models;      // Your local API folder models (ProductViewModel)
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
        public ActionResult<IEnumerable<Agullto_IMS.Models.Product>> GetAllProducts()
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
        [HttpGet("{id:guid}")]
        public ActionResult<Agullto_IMS.Models.Product> GetProductById(Guid id)
        {
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
        public ActionResult<List<Agullto_IMS.Models.Product>> GetLowStockItems()
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
        // Integrates ProductViewModel from your local models folder
        [HttpPost]
        public ActionResult<Agullto_IMS.Models.Product> CreateProduct([FromBody] ProductViewModel model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Product data payload cannot be null." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Map the local ViewModel to your core data model type
                var newProduct = new Agullto_IMS.Models.Product
                {
                    Id = Guid.NewGuid(), // Server explicitly controls the new Guid generation
                    Name = model.Name,
                    Stock = model.Stock,
                    Department = model.Department,
                    WeightValue = model.WeightValue,
                    Unit = model.Unit,
                    CostPrice = model.CostPrice,
                    SellingPrice = model.SellingPrice,
                    Location = model.Location,
                    ExpirationDate = model.ExpirationDate
                };

                _productService.AddProduct(newProduct);
                return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
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

        // 5. UPDATE PRODUCT -> PUT: api/inventorymanagement/{id}
        // Accepts the target ID in the route URL, and properties via the input ViewModel
        [HttpPut("{id:guid}")]
        public IActionResult UpdateProduct(Guid id, [FromBody] ProductViewModel model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Product update data payload cannot be null." });
            }

            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "A valid Product ID must be provided within the URL route for updating." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Map the updated values onto your core data structure explicitly targeting the route ID
                var updatedProduct = new Agullto_IMS.Models.Product
                {
                    Id = id,
                    Name = model.Name,
                    Stock = model.Stock,
                    Department = model.Department,
                    WeightValue = model.WeightValue,
                    Unit = model.Unit,
                    CostPrice = model.CostPrice,
                    SellingPrice = model.SellingPrice,
                    Location = model.Location,
                    ExpirationDate = model.ExpirationDate
                };

                bool wasUpdated = _productService.UpdateProduct(updatedProduct);
                if (!wasUpdated)
                {
                    return NotFound(new { message = $"Failed to update. Product with ID {id} not found." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateProduct for ID {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while updating the product entry.",
                    error = ex.Message
                });
            }
        }

        // 6. DELETE PRODUCT -> DELETE: api/inventorymanagement/{id}
        [HttpDelete("{id:guid}")]
        public IActionResult DeleteProduct(Guid id)
        {
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