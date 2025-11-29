using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Interfaces;
using OrderManagementSystem.DTOs;

namespace OrderManagementSystem.Core.Services;

public class ProductManagementService : IProductManagementService
{
    private readonly IProductRepository _productRepository;

    public ProductManagementService(IProductRepository productRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<ServiceResult<List<Product>>> GetAllProductsAsync()
    {
        try
        {
            var products = await _productRepository.GetAllAsync();
            return ServiceResult<List<Product>>.Success(products);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<Product>>.Failure($"Failed to retrieve products: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult<List<Product>>> GetActiveProductsAsync()
    {
        try
        {
            var products = await _productRepository.GetActiveProductsAsync();
            return ServiceResult<List<Product>>.Success(products);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<Product>>.Failure($"Failed to retrieve active products: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult<List<Product>>> GetActiveProductsByCategoryAsync(ProductCategory category)
    {
        try
        {
            var products = await _productRepository.GetActiveProductsByCategoryAsync(category);
            return ServiceResult<List<Product>>.Success(products);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<Product>>.Failure($"Failed to retrieve active products by category: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult<Product?>> GetProductByIdAsync(int id)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            return ServiceResult<Product?>.Success(product);
        }
        catch (Exception ex)
        {
            return ServiceResult<Product?>.Failure($"Failed to retrieve product: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult<Product>> CreateProductAsync(CreateProductRequest request)
    {
        try
        {
            var product = new Product(request.Name, request.Description, request.Price, request.StockQuantity, request.Category);
            var createdProduct = await _productRepository.CreateAsync(product);
            return ServiceResult<Product>.Success(createdProduct);
        }
        catch (Exception ex)
        {
            return ServiceResult<Product>.Failure($"Failed to create product: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult<bool>> UpdateProductAsync(UpdateProductRequest request)
    {
        try
        {
            var existingProduct = await _productRepository.GetByIdAsync(request.Id);
            if (existingProduct == null)
                return ServiceResult<bool>.Failure("Product not found");

            existingProduct.UpdateDetails(request.Name, request.Description, request.Price, request.Category);
            existingProduct.SetStock(request.StockQuantity);

            var success = await _productRepository.UpdateAsync(existingProduct);
            return ServiceResult<bool>.Success(success);
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Failed to update product: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult<bool>> DeleteProductAsync(int id)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return ServiceResult<bool>.Failure("Product not found");

            product.Deactivate();
            var success = await _productRepository.UpdateAsync(product);
            return ServiceResult<bool>.Success(success);
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Failed to delete product: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult<bool>> UpdateProductStockAsync(int productId, int quantity)
    {
        try
        {
            var success = await _productRepository.UpdateStockAsync(productId, quantity);
            return ServiceResult<bool>.Success(success);
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Failed to update product stock: {ex.Message}", ex);
        }
    }
}
