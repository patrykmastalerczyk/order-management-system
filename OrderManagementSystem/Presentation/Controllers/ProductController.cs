using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Interfaces;
using OrderManagementSystem.DTOs;

namespace OrderManagementSystem.Presentation.Controllers;

public class ProductController
{
    private readonly IProductManagementService _productService;

    public ProductController(IProductManagementService productService)
    {
        _productService = productService;
    }

    public async Task<List<ProductDto>> GetAllProductsAsync()
    {
        var result = await _productService.GetAllProductsAsync();
        if (!result.IsSuccess)
            throw new InvalidOperationException(result.ErrorMessage);
        
        return result.Data!.Select(MapToDto).ToList();
    }

    public async Task<List<ProductDto>> GetActiveProductsAsync()
    {
        var result = await _productService.GetActiveProductsAsync();
        if (!result.IsSuccess)
            throw new InvalidOperationException(result.ErrorMessage);
        
        return result.Data!.Select(MapToDto).ToList();
    }

    public async Task<List<ProductDto>> GetActiveProductsByCategoryAsync(ProductCategory category)
    {
        var result = await _productService.GetActiveProductsByCategoryAsync(category);
        if (!result.IsSuccess)
            throw new InvalidOperationException(result.ErrorMessage);
        
        return result.Data!.Select(MapToDto).ToList();
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var result = await _productService.GetProductByIdAsync(id);
        if (!result.IsSuccess)
            throw new InvalidOperationException(result.ErrorMessage);
        
        return result.Data != null ? MapToDto(result.Data) : null;
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request)
    {
        var result = await _productService.CreateProductAsync(request);
        if (!result.IsSuccess)
            throw new InvalidOperationException(result.ErrorMessage);
        
        return MapToDto(result.Data!);
    }

    public async Task<bool> UpdateProductAsync(UpdateProductRequest request)
    {
        var result = await _productService.UpdateProductAsync(request);
        if (!result.IsSuccess)
            throw new InvalidOperationException(result.ErrorMessage);
        
        return result.Data;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var result = await _productService.DeleteProductAsync(id);
        if (!result.IsSuccess)
            throw new InvalidOperationException(result.ErrorMessage);
        
        return result.Data;
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            Category = product.Category.ToString(),
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
