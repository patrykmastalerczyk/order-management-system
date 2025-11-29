using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Interfaces;
using OrderManagementSystem.DTOs;

namespace OrderManagementSystem.Core.Services;

public class OrderManagementService : IOrderManagementService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public OrderManagementService(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<ServiceResult<List<Order>>> GetAllOrdersAsync()
    {
        try
        {
            var orders = await _orderRepository.GetAllAsync();
            return ServiceResult<List<Order>>.Success(orders);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<Order>>.Failure($"Failed to retrieve orders: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult<List<Order>>> GetOrdersByStatusAsync(OrderStatus status)
    {
        try
        {
            var orders = await _orderRepository.GetByStatusAsync(status);
            return ServiceResult<List<Order>>.Success(orders);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<Order>>.Failure($"Failed to retrieve orders by status: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult<Order?>> GetOrderByIdAsync(int id)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return ServiceResult<Order?>.Success(order);
        }
        catch (Exception ex)
        {
            return ServiceResult<Order?>.Failure($"Failed to retrieve order: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult<Order?>> GetOrderByNumberAsync(string orderNumber)
    {
        try
        {
            var order = await _orderRepository.GetByOrderNumberAsync(orderNumber);
            return ServiceResult<Order?>.Success(order);
        }
        catch (Exception ex)
        {
            return ServiceResult<Order?>.Failure($"Failed to retrieve order by number: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult<Order>> CreateOrderAsync(CreateOrderRequest request)
    {
        try
        {
            var order = new Order(request.CustomerName, request.CustomerEmail);

            foreach (var itemDto in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
                if (product == null)
                    return ServiceResult<Order>.Failure($"Product with ID {itemDto.ProductId} not found");

                if (!product.CanReserve(itemDto.Quantity))
                    return ServiceResult<Order>.Failure($"Product '{product.Name}' is not available in requested quantity");

                order.AddItem(product, itemDto.Quantity);
                if (product.Category != ProductCategory.Coffee)
                {
                    await _productRepository.UpdateStockAsync(itemDto.ProductId, -itemDto.Quantity);
                }
            }

            var createdOrder = await _orderRepository.CreateAsync(order);
            return ServiceResult<Order>.Success(createdOrder);
        }
        catch (Exception ex)
        {
            var inner = ex.InnerException?.Message;
            var detailed = inner != null ? $"{ex.Message} | Inner: {inner}" : ex.Message;
            return ServiceResult<Order>.Failure($"Failed to create order: {detailed}", ex);
        }
    }

    public async Task<ServiceResult<bool>> UpdateOrderStatusAsync(UpdateOrderStatusRequest request)
    {
        try
        {
            if (!Enum.TryParse<OrderStatus>(request.Status, out var status))
                return ServiceResult<bool>.Failure("Invalid order status");

            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
                return ServiceResult<bool>.Failure("Order not found");

            order.UpdateStatus(status);
            var success = await _orderRepository.UpdateAsync(order);
            return ServiceResult<bool>.Success(success);
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Failed to update order status: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult<bool>> CancelOrderAsync(int orderId)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return ServiceResult<bool>.Failure("Order not found");

            if (!order.CanBeCancelled())
                return ServiceResult<bool>.Failure("Order cannot be cancelled in current status");

            foreach (var item in order.Items)
            {
                if (item.Product.Category != ProductCategory.Coffee)
                {
                    await _productRepository.UpdateStockAsync(item.ProductId, item.Quantity);
                }
            }

            order.UpdateStatus(OrderStatus.Cancelled);
            var success = await _orderRepository.UpdateAsync(order);
            return ServiceResult<bool>.Success(success);
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Failed to cancel order: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult<List<Order>>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var orders = await _orderRepository.GetByDateRangeAsync(startDate, endDate);
            return ServiceResult<List<Order>>.Success(orders);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<Order>>.Failure($"Failed to retrieve orders by date range: {ex.Message}", ex);
        }
    }
}
