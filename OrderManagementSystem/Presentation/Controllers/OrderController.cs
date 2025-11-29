using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Interfaces;
using OrderManagementSystem.DTOs;

namespace OrderManagementSystem.Presentation.Controllers;

public class OrderController
{
    private readonly IOrderManagementService _orderService;

    public OrderController(IOrderManagementService orderService)
    {
        _orderService = orderService;
    }

    public async Task<List<OrderDto>> GetAllOrdersAsync()
    {
        var result = await _orderService.GetAllOrdersAsync();
        if (!result.IsSuccess)
            throw new InvalidOperationException(result.ErrorMessage);
        
        return result.Data!.Select(MapToDto).ToList();
    }

    public async Task<List<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
    {
        var result = await _orderService.GetOrdersByStatusAsync(status);
        if (!result.IsSuccess)
            throw new InvalidOperationException(result.ErrorMessage);
        
        return result.Data!.Select(MapToDto).ToList();
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        var result = await _orderService.GetOrderByIdAsync(id);
        if (!result.IsSuccess)
            throw new InvalidOperationException(result.ErrorMessage);
        
        return result.Data != null ? MapToDto(result.Data) : null;
    }

    public async Task<OrderDto?> GetOrderByNumberAsync(string orderNumber)
    {
        var result = await _orderService.GetOrderByNumberAsync(orderNumber);
        if (!result.IsSuccess)
            throw new InvalidOperationException(result.ErrorMessage);
        
        return result.Data != null ? MapToDto(result.Data) : null;
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderRequest request)
    {
        var result = await _orderService.CreateOrderAsync(request);
        if (!result.IsSuccess)
            throw new InvalidOperationException(result.ErrorMessage);
        
        return MapToDto(result.Data!);
    }

    public async Task<bool> UpdateOrderStatusAsync(UpdateOrderStatusRequest request)
    {
        var result = await _orderService.UpdateOrderStatusAsync(request);
        if (!result.IsSuccess)
            throw new InvalidOperationException(result.ErrorMessage);
        
        return result.Data;
    }

    public async Task<bool> CancelOrderAsync(int orderId)
    {
        var result = await _orderService.CancelOrderAsync(orderId);
        if (!result.IsSuccess)
            throw new InvalidOperationException(result.ErrorMessage);
        
        return result.Data;
    }

    public async Task<List<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var result = await _orderService.GetOrdersByDateRangeAsync(startDate, endDate);
        if (!result.IsSuccess)
            throw new InvalidOperationException(result.ErrorMessage);
        
        return result.Data!.Select(MapToDto).ToList();
    }

    public async Task<ReceiptDto> GenerateReceiptAsync(int orderId)
    {
        var result = await _orderService.GetOrderByIdAsync(orderId);
        if (!result.IsSuccess || result.Data == null)
        {
            throw new ArgumentException("Zamówienie nie zostało znalezione");
        }

        return new ReceiptDto
        {
            OrderNumber = result.Data.OrderNumber,
            CreatedAt = result.Data.CreatedAt,
            CustomerName = result.Data.CustomerName,
            CustomerEmail = result.Data.CustomerEmail,
            Items = result.Data.Items.Select(MapItemToDto).ToList(),
            TotalAmount = result.Data.TotalAmount
        };
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            Items = order.Items.Select(MapItemToDto).ToList()
        };
    }

    private static OrderItemDto MapItemToDto(OrderItem item)
    {
        return new OrderItemDto
        {
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice
        };
    }
}
