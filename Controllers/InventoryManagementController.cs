using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Agullto_IMS.Models;   
using Agullto_IMS.Services; 
using System;
using System.Collections.Generic;

namespace Agullto_IMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryManagementController : ControllerBase
    {
        private readonly ProductService _productService;

        // The .NET runtime automatically handles dependency injection across your project reference
        public InventoryManagementController(ProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        // 1. GET ALL PRODUCTS -> GET: api/inventorymanagement
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetAllProducts()
        {
            var products = _productService.GetAllProducts();
            return Ok(products); // Returns 200 OK with the product array
        }

        // 2. GET PRODUCT BY ID -> GET: api/inventorymanagement/{id}
        [HttpGet("{id}")]
        public ActionResult<Product> GetProductById(Guid id)
        {
            var product = _productService.FindProduct(id);
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {id} was not found." }); // Returns 404
            }

            return Ok(product); // Returns 200 OK
        }

        // 3. CREATE PRODUCT -> POST: api/inventorymanagement
        [HttpPost]
        public ActionResult<Product> CreateProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest(new { message = "Product data cannot be null." }); // Returns 400
            }

            // Simple Model Validation Check
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _productService.AddProduct(product);

            // Returns 201 Created status code along with a URL pointing to the new resource
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        // 4. UPDATE PRODUCT -> PUT: api/inventorymanagement
        [HttpPut]
        public IActionResult UpdateProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest(new { message = "Product data cannot be null." });
            }

            bool wasUpdated = _productService.UpdateProduct(product);
            if (!wasUpdated)
            {
                return NotFound(new { message = $"Failed to update. Product with ID {product.Id} not found." });
            }

            return NoContent(); // Returns 204 NoContent (Standard for successful REST updates)
        }

        // 5. DELETE PRODUCT -> DELETE: api/inventorymanagement/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(Guid id)
        {
            bool wasDeleted = _productService.DeleteProduct(id);
            if (!wasDeleted)
            {
                return NotFound(new { message = $"Failed to delete. Product with ID {id} not found." });
            }

            return Ok(new { message = "Product deleted successfully from the inventory system." }); // Returns 200 OK
        }
    }
}