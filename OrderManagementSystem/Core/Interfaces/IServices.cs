using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.DTOs;

namespace OrderManagementSystem.Core.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResult> LoginAsync(string username, string password);
    void Logout();
    User? GetCurrentUser();
    bool IsAuthenticated();
    Task<bool> ValidateCredentialsAsync(string username, string password);
}

public interface IProductManagementService
{
    Task<ServiceResult<List<Product>>> GetAllProductsAsync();
    Task<ServiceResult<List<Product>>> GetActiveProductsAsync();
    Task<ServiceResult<List<Product>>> GetActiveProductsByCategoryAsync(ProductCategory category);
    Task<ServiceResult<Product?>> GetProductByIdAsync(int id);
    Task<ServiceResult<Product>> CreateProductAsync(CreateProductRequest request);
    Task<ServiceResult<bool>> UpdateProductAsync(UpdateProductRequest request);
    Task<ServiceResult<bool>> DeleteProductAsync(int id);
    Task<ServiceResult<bool>> UpdateProductStockAsync(int productId, int quantity);
}

public interface IOrderManagementService
{
    Task<ServiceResult<List<Order>>> GetAllOrdersAsync();
    Task<ServiceResult<List<Order>>> GetOrdersByStatusAsync(OrderStatus status);
    Task<ServiceResult<Order?>> GetOrderByIdAsync(int id);
    Task<ServiceResult<Order?>> GetOrderByNumberAsync(string orderNumber);
    Task<ServiceResult<Order>> CreateOrderAsync(CreateOrderRequest request);
    Task<ServiceResult<bool>> UpdateOrderStatusAsync(UpdateOrderStatusRequest request);
    Task<ServiceResult<bool>> CancelOrderAsync(int orderId);
    Task<ServiceResult<List<Order>>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
}

public interface IShoppingCartService
{
    void AddItem(Product product, int quantity);
    void RemoveItem(int productId);
    void UpdateQuantity(int productId, int quantity);
    void ClearCart();
    IReadOnlyList<OrderItem> GetCartItems();
    decimal GetTotalAmount();
    int GetItemCount();
    bool IsEmpty { get; }
    bool ContainsProduct(int productId);
    int GetProductQuantity(int productId);
}

public interface IReceiptService
{
    Task<ServiceResult<ReceiptDto>> GenerateReceiptAsync(int orderId);
    Task<ServiceResult<string>> GenerateReceiptTextAsync(int orderId);
}

public class ServiceResult<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? ErrorMessage { get; }
    public Exception? Exception { get; }

    private ServiceResult(bool isSuccess, T? data, string? errorMessage, Exception? exception = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
        Exception = exception;
    }

    public static ServiceResult<T> Success(T data) => new(true, data, null);
    public static ServiceResult<T> Failure(string errorMessage) => new(false, default, errorMessage);
    public static ServiceResult<T> Failure(string errorMessage, Exception exception) => new(false, default, errorMessage, exception);
}

public class AuthenticationResult
{
    public bool IsSuccess { get; }
    public User? User { get; }
    public string? ErrorMessage { get; }

    private AuthenticationResult(bool isSuccess, User? user, string? errorMessage)
    {
        IsSuccess = isSuccess;
        User = user;
        ErrorMessage = errorMessage;
    }

    public static AuthenticationResult Success(User user) => new(true, user, null);
    public static AuthenticationResult Failure(string errorMessage) => new(false, null, errorMessage);
}
