using OrderManagementSystem.Core.Entities;

namespace OrderManagementSystem.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByUsernameAndPasswordAsync(string username, string password);
    Task<List<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
}

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> GetActiveProductsAsync();
    Task<List<Product>> GetActiveProductsByCategoryAsync(ProductCategory category);
    Task<Product> CreateAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateStockAsync(int productId, int quantity);
}

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<Order?> GetByOrderNumberAsync(string orderNumber);
    Task<List<Order>> GetAllAsync();
    Task<List<Order>> GetByStatusAsync(OrderStatus status);
    Task<List<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Order> CreateAsync(Order order);
    Task<bool> UpdateAsync(Order order);
    Task<bool> UpdateStatusAsync(int orderId, OrderStatus status);
    Task<bool> DeleteAsync(int id);
}