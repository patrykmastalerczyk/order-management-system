namespace OrderManagementSystem.Core.Entities;

public class Order
{
    private readonly List<OrderItem> _items = new();

    public int Id { get; private set; }
    public string OrderNumber { get; private set; }
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount => _items.Sum(item => item.TotalPrice);
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? CustomerName { get; private set; }
    public string? CustomerEmail { get; private set; }

    private Order()
    {
        OrderNumber = string.Empty;
    }

    public Order(string? customerName = null, string? customerEmail = null)
    {
        OrderNumber = GenerateOrderNumber();
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        CustomerName = customerName;
        CustomerEmail = customerEmail;
    }

    public void AddItem(Product product, int quantity)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
        if (!product.CanReserve(quantity))
            throw new InvalidOperationException($"Product '{product.Name}' is not available in requested quantity");

        var existingItem = _items.FirstOrDefault(item => item.ProductId == product.Id);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var orderItem = new OrderItem(product.Id, product, quantity, product.Price);
            _items.Add(orderItem);
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveItem(int productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            _items.Remove(item);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void UpdateItemQuantity(int productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            throw new InvalidOperationException("Item not found in order");

        item.UpdateQuantity(quantity);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        if (Status == OrderStatus.Completed && newStatus != OrderStatus.Completed)
            throw new InvalidOperationException("Cannot change status of completed order");
        if (Status == OrderStatus.Cancelled && newStatus != OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot change status of cancelled order");

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCustomerInfo(string? customerName, string? customerEmail)
    {
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddExistingItem(OrderItem orderItem)
    {
        if (orderItem == null)
            throw new ArgumentNullException(nameof(orderItem));
        
        _items.Add(orderItem);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool CanBeCancelled()
    {
        return Status == OrderStatus.Pending || Status == OrderStatus.InProgress;
    }

    public bool CanBeCompleted()
    {
        return Status == OrderStatus.InProgress || Status == OrderStatus.Shipped;
    }

    private static string GenerateOrderNumber()
    {
        var guidPart = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{guidPart}";
    }

    public void RegenerateOrderNumber()
    {
        var newNumber = GenerateOrderNumber();
        var prop = typeof(Order).GetProperty("OrderNumber");
        prop?.SetValue(this, newNumber);
        UpdatedAt = DateTime.UtcNow;
    }

    public static Order CreateWithId(int id, string orderNumber, string? customerName = null, string? customerEmail = null)
    {
        var order = new Order(customerName, customerEmail);
        var idProperty = typeof(Order).GetProperty("Id");
        var orderNumberProperty = typeof(Order).GetProperty("OrderNumber");
        idProperty?.SetValue(order, id);
        orderNumberProperty?.SetValue(order, orderNumber);
        return order;
    }
}

public class OrderItem
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public int ProductId { get; private set; }
    public Product Product { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;

    private OrderItem()
    {
        Product = null!;
    }

    public OrderItem(int productId, Product product, int quantity, decimal unitPrice)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        ProductId = productId;
        Product = product;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        Quantity = quantity;
    }

    public static OrderItem CreateWithId(int id, int orderId, int productId, Product product, int quantity, decimal unitPrice)
    {
        var orderItem = new OrderItem(productId, product, quantity, unitPrice);
        var idProperty = typeof(OrderItem).GetProperty("Id");
        var orderIdProperty = typeof(OrderItem).GetProperty("OrderId");
        idProperty?.SetValue(orderItem, id);
        orderIdProperty?.SetValue(orderItem, orderId);
        return orderItem;
    }
}

public enum OrderStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3,
    Shipped = 4
}
