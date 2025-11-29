using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Interfaces;

namespace OrderManagementSystem.Infrastructure.Data;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products.Where(p => p.IsActive).ToListAsync();
    }

    public async Task<List<Product>> GetActiveProductsAsync()
    {
        return await _context.Products
            .Where(p => p.IsActive && (p.Category == ProductCategory.Coffee || p.StockQuantity > 0))
            .ToListAsync();
    }

    public async Task<List<Product>> GetActiveProductsByCategoryAsync(ProductCategory category)
    {
        if (category == ProductCategory.Coffee)
        {
            return await _context.Products.Where(p => p.IsActive && p.Category == category).ToListAsync();
        }

        return await _context.Products.Where(p => p.IsActive && p.Category == category && p.StockQuantity > 0).ToListAsync();
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
        if (existingProduct == null) return false;

        existingProduct.UpdateDetails(product.Name, product.Description, product.Price, product.Category);
        existingProduct.SetStock(product.StockQuantity);
        if (product.IsActive != existingProduct.IsActive)
        {
            if (product.IsActive)
                existingProduct.Activate();
            else
                existingProduct.Deactivate();
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return false;

        product.Deactivate();
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateStockAsync(int productId, int quantity)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null) return false;

        product.UpdateStock(quantity);
        await _context.SaveChangesAsync();
        return true;
    }
}
