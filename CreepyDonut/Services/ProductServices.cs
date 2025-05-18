using Microsoft.EntityFrameworkCore;
using CreepyDonut.Data;
using CreepyDonut.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreepyDonut.DTO;

namespace CreepyDonut.Services
{
    public class ProductService
    {
        private readonly ApiContext _context;

        public ProductService(ApiContext context)
        {
            _context = context;
        }

        // GET ALL PRODUCTS
        public async Task<List<ProductResponseDTO>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Select(p => new ProductResponseDTO
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    Quantity = p.Quantity,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name
                })
                .ToListAsync();
        }

        // GET PRODUCT BY ID
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        // CREATE NEW PRODUCT
        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        // UPDATE PRODUCT
        public async Task<bool> UpdateAsync(int id, Product updatedProduct)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
                return false;

            existingProduct.Name = updatedProduct.Name;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.ImageUrl = updatedProduct.ImageUrl;
            existingProduct.Quantity = updatedProduct.Quantity;

            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE PRODUCT
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
