﻿using Microsoft.AspNetCore.Mvc;
using CreepyDonut.Models;
using CreepyDonut.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreepyDonut.DTO;

namespace CreepyDonut.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        // GET ALL PRODUCTS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        // GET PRODUCT BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            return Ok(product);
        }

        // CREATE NEW PRODUCT
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NewProductDTO newProductDto)
        {
            var product = new Product
            {
                Name = newProductDto.Name,
                Description = newProductDto.Description,
                Price = newProductDto.Price,
                ImageUrl = newProductDto.ImageUrl,
                Quantity = newProductDto.Quantity,
                CategoryId = newProductDto.CategoryId
            };

            var createdProduct = await _productService.CreateAsync(product);

            return CreatedAtAction(nameof(GetById), new { id = createdProduct.ProductId }, createdProduct);
        }

        [HttpGet("search/{name}")]
        public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> SearchByName(string name)
        {
            var products = await _productService.GetAllAsync(); // This simulates access to the product list

            var matchingProducts = products
                .Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .Select(p => new ProductResponseDTO
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    Quantity = p.Quantity,
                    CategoryId = p.CategoryId,
                    CategoryName = "Example Category" // Replace with actual logic if needed
                })
                .ToList();

            if (!matchingProducts.Any())
                return NotFound(new { message = "No matching products found" });

            return Ok(matchingProducts);
        }




        // UPDATE PRODUCT
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product updatedProduct)
        {
            var success = await _productService.UpdateAsync(id, updatedProduct);
            if (!success)
                return NotFound(new { message = "Product not found" });

            return Ok(new { message = "Product updated successfully" });
        }

        // DELETE PRODUCT
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _productService.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = "Product not found" });

            return Ok(new { message = "Product deleted successfully" });
        }
    }
}
