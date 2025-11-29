using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Interfaces;

namespace OrderManagementSystem.Core.Services;

public class ShoppingCartService : IShoppingCartService
{
    private readonly List<OrderItem> _cartItems = new();

    public IReadOnlyList<OrderItem> GetCartItems() => _cartItems.AsReadOnly();
    public decimal GetTotalAmount() => _cartItems.Sum(item => item.TotalPrice);
    public int GetItemCount() => _cartItems.Sum(item => item.Quantity);
    public bool IsEmpty => !_cartItems.Any();

    public void AddItem(Product product, int quantity)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (product.Category != ProductCategory.Coffee && !product.IsAvailable(quantity))
            throw new InvalidOperationException($"Product '{product.Name}' is not available in requested quantity");

        var existingItem = _cartItems.FirstOrDefault(item => item.ProductId == product.Id);

        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + quantity;
            if (product.Category != ProductCategory.Coffee && !product.IsAvailable(newQuantity))
                throw new InvalidOperationException($"Product '{product.Name}' is not available in requested total quantity");

            existingItem.UpdateQuantity(newQuantity);
        }
        else
        {
            var orderItem = new OrderItem(product.Id, product, quantity, product.Price);
            _cartItems.Add(orderItem);
        }
    }

    public void RemoveItem(int productId)
    {
        var item = _cartItems.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            _cartItems.Remove(item);
        }
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        if (quantity <= 0)
        {
            RemoveItem(productId);
            return;
        }

        var item = _cartItems.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            throw new InvalidOperationException("Item not found in cart");

        if (item.Product.Category != ProductCategory.Coffee && !item.Product.IsAvailable(quantity))
            throw new InvalidOperationException($"Product '{item.Product.Name}' is not available in requested quantity");

        item.UpdateQuantity(quantity);
    }

    public void ClearCart()
    {
        _cartItems.Clear();
    }

    public bool ContainsProduct(int productId)
    {
        return _cartItems.Any(item => item.ProductId == productId);
    }

    public int GetProductQuantity(int productId)
    {
        var item = _cartItems.FirstOrDefault(i => i.ProductId == productId);
        return item?.Quantity ?? 0;
    }
}
