using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Interfaces;

namespace OrderManagementSystem.Infrastructure.Data;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
    {
        return await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(item => item.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Order>> GetByStatusAsync(OrderStatus status)
    {
        return await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(item => item.Product)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(item => item.Product)
            .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order> CreateAsync(Order order)
    {
        var productIds = order.Items.Select(i => i.ProductId).Distinct().ToList();

        var trackedProducts = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        foreach (var productId in productIds)
        {
            if (!trackedProducts.ContainsKey(productId))
            {
                throw new InvalidOperationException($"Product with ID {productId} not found in database");
            }
        }

        foreach (var trackedProduct in trackedProducts.Values)
        {
            var productEntry = _context.Entry(trackedProduct);
            if (productEntry.State == EntityState.Detached)
            {
                _context.Products.Attach(trackedProduct);
            }
            productEntry.State = EntityState.Unchanged;
        }

        var itemsField = typeof(Order).GetField("_items",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (itemsField != null && itemsField.GetValue(order) is List<OrderItem> itemsList)
        {
            var oldItems = itemsList.ToList();
            itemsList.Clear();

            foreach (var oldItem in oldItems)
            {
                if (trackedProducts.TryGetValue(oldItem.ProductId, out var trackedProduct))
                {
                    var newOrderItem = new OrderItem(
                        oldItem.ProductId,
                        trackedProduct,
                        oldItem.Quantity,
                        oldItem.UnitPrice
                    );
                    itemsList.Add(newOrderItem);
                }
            }
        }
        else
        {
            throw new InvalidOperationException("Could not access Order items collection");
        }

        _context.Orders.Add(order);

        var maxRetries = 5;
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await _context.SaveChangesAsync();
                return order;
            }
            catch (DbUpdateException dbEx)
            {
                var innerMsg = dbEx.InnerException?.Message ?? dbEx.Message;
                var isUniqueOrderNumber = innerMsg.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase)
                    && innerMsg.Contains("OrderNumber", StringComparison.OrdinalIgnoreCase);

                if (isUniqueOrderNumber && attempt < maxRetries)
                {
                    order.RegenerateOrderNumber();
                    continue;
                }

                throw;
            }
        }

        return order;
    }

    public async Task<bool> UpdateAsync(Order order)
    {
        var existingOrder = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == order.Id);
        
        if (existingOrder == null) return false;

        existingOrder.UpdateStatus(order.Status);
        existingOrder.UpdateCustomerInfo(order.CustomerName, order.CustomerEmail);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateStatusAsync(int orderId, OrderStatus status)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null) return false;

        order.UpdateStatus(status);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }
}
