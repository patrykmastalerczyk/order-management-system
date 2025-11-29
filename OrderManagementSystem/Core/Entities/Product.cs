namespace OrderManagementSystem.Core.Entities;

public enum ProductCategory
{
    Coffee = 0,
    Bagels = 1,
    Croissants = 2,
    Cakes = 3
}

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }
    public ProductCategory Category { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Product()
    {
        Name = string.Empty;
        Description = string.Empty;
    }

    public Product(string name, string description, decimal price, int stockQuantity, ProductCategory category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be null or empty", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Product description cannot be null or empty", nameof(description));
        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));
        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(stockQuantity));

        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        Category = category;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string description, decimal price, ProductCategory category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be null or empty", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Product description cannot be null or empty", nameof(description));
        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        Name = name;
        Description = description;
        Price = price;
        Category = category;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStock(int quantity)
    {
        if (StockQuantity + quantity < 0)
            throw new InvalidOperationException("Insufficient stock quantity");

        StockQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStock(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(quantity));

        StockQuantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsAvailable(int requestedQuantity = 1)
    {
        if (!IsActive)
            return false;
        
        if (Category == ProductCategory.Coffee)
            return true;
        
        return StockQuantity >= requestedQuantity;
    }

    public bool CanReserve(int quantity)
    {
        if (!IsActive)
            return false;
        
        if (Category == ProductCategory.Coffee)
            return true;
        
        return StockQuantity >= quantity;
    }

    public static Product CreateWithId(int id, string name, string description, decimal price, int stockQuantity, ProductCategory category)
    {
        var product = new Product(name, description, price, stockQuantity, category);
        var idProperty = typeof(Product).GetProperty("Id");
        idProperty?.SetValue(product, id);
        return product;
    }
}
