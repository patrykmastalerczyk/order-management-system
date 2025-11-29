using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Interfaces;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Presentation.Controllers;

namespace OrderManagementSystem.Presentation.Views;

public class CustomerShoppingView
{
    private readonly ProductController _productController;
    private readonly OrderController _orderController;
    private readonly IShoppingCartService _cartService;

    public CustomerShoppingView(
        ProductController productController,
        OrderController orderController,
        IShoppingCartService cartService)
    {
        _productController = productController;
        _orderController = orderController;
        _cartService = cartService;
    }

    public async Task DisplayAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== SKŁADANIE ZAMÓWIENIA ===");
            Console.WriteLine($"Koszyk: {_cartService.GetItemCount()} produktów | Wartość: {_cartService.GetTotalAmount():C}");
            Console.WriteLine("1. Przeglądaj produkty");
            Console.WriteLine("2. Wyświetl koszyk");
            Console.WriteLine("3. Usuń produkt z koszyka");
            Console.WriteLine("4. Złóż zamówienie");
            Console.WriteLine("5. Wróć");
            Console.WriteLine("===========================");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await BrowseProductsAsync();
                    break;
                case "2":
                    DisplayCart();
                    break;
                case "3":
                    RemoveFromCart();
                    break;
                case "4":
                    await PlaceOrderAsync();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Nieprawidłowy wybór.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private async Task BrowseProductsAsync()
    {
        Console.Clear();
        Console.WriteLine("=== WYBÓR KATEGORII ===");
        Console.WriteLine("1. Kawa (Coffee)");
        Console.WriteLine("2. Bajgle (Bagels)");
        Console.WriteLine("3. Rogale (Croissants)");
        Console.WriteLine("4. Ciasta (Cakes)");
        Console.WriteLine("5. Wszystkie kategorie");
        Console.Write("\nWybierz kategorię (1-5): ");
        
        if (!int.TryParse(Console.ReadLine(), out var categoryChoice) || categoryChoice < 1 || categoryChoice > 5)
        {
            Console.WriteLine("Nieprawidłowy wybór!");
            Console.ReadKey();
            return;
        }

        ProductCategory? selectedCategory = categoryChoice switch
        {
            1 => ProductCategory.Coffee,
            2 => ProductCategory.Bagels,
            3 => ProductCategory.Croissants,
            4 => ProductCategory.Cakes,
            5 => null,
            _ => null
        };

        Console.Clear();
        string categoryName = selectedCategory.HasValue ? selectedCategory.Value.ToString() : "Wszystkie";
        Console.WriteLine($"=== DOSTĘPNE PRODUKTY - {categoryName} ===");

        List<ProductDto> products;
        if (selectedCategory.HasValue)
        {
            products = await _productController.GetActiveProductsByCategoryAsync(selectedCategory.Value);
        }
        else
        {
            products = await _productController.GetActiveProductsAsync();
        }

        if (!products.Any())
        {
            Console.WriteLine($"Brak dostępnych produktów w kategorii: {categoryName}.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"{"ID",-4} {"Nazwa",-20} {"Opis",-30} {"Cena",-10} {"Stan",-8}");
        Console.WriteLine(new string('-', 80));

        foreach (var product in products)
        {
            string stockDisplay = product.Category == "Coffee" ? "Nielimit." : product.StockQuantity.ToString();
            Console.WriteLine($"{product.Id,-4} {product.Name,-20} {product.Description,-30} {product.Price,-10:C} {stockDisplay,-8}");
        }

        Console.WriteLine("\nWybierz produkt do dodania do koszyka:");
        Console.Write("ID produktu: ");

        if (!int.TryParse(Console.ReadLine(), out var productId))
        {
            Console.WriteLine("Nieprawidłowe ID!");
            Console.ReadKey();
            return;
        }

        var selectedProduct = products.FirstOrDefault(p => p.Id == productId);
        if (selectedProduct == null)
        {
            Console.WriteLine("Produkt o podanym ID nie istnieje!");
            Console.ReadKey();
            return;
        }

        Console.Write("Ilość: ");
        if (!int.TryParse(Console.ReadLine(), out var quantity) || quantity <= 0)
        {
            Console.WriteLine("Nieprawidłowa ilość!");
            Console.ReadKey();
            return;
        }

        if (selectedProduct.Category != "Coffee" && quantity > selectedProduct.StockQuantity)
        {
            Console.WriteLine($"Dostępna ilość: {selectedProduct.StockQuantity}");
            Console.ReadKey();
            return;
        }

        if (!Enum.TryParse<ProductCategory>(selectedProduct.Category, out var category))
        {
            category = ProductCategory.Coffee;
        }
        
        var productToAdd = Product.CreateWithId(
            selectedProduct.Id,
            selectedProduct.Name,
            selectedProduct.Description,
            selectedProduct.Price,
            selectedProduct.StockQuantity,
            category
        );

        _cartService.AddItem(productToAdd, quantity);
        Console.WriteLine($"Dodano '{selectedProduct.Name}' do koszyka!");
        Console.ReadKey();
    }

    private void DisplayCart()
    {
        Console.Clear();
        Console.WriteLine("=== ZAWARTOŚĆ KOSZYKA ===");
        
        var cartItems = _cartService.GetCartItems();
        
        if (!cartItems.Any())
        {
            Console.WriteLine("Koszyk jest pusty.");
        }
        else
        {
            Console.WriteLine($"{"Produkt",-20} {"Ilość",-8} {"Cena jednostkowa",-15} {"Wartość",-10}");
            Console.WriteLine(new string('-', 60));
            
            foreach (var item in cartItems)
            {
                Console.WriteLine($"{item.Product.Name,-20} {item.Quantity,-8} {item.UnitPrice,-15:C} {item.TotalPrice,-10:C}");
            }
            
            Console.WriteLine(new string('-', 60));
            Console.WriteLine($"Łączna wartość: {_cartService.GetTotalAmount(),-40:C}");
        }
        
        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    private void RemoveFromCart()
    {
        Console.Clear();
        Console.WriteLine("=== USUWANIE PRODUKTU Z KOSZYKA ===");
        
        var cartItems = _cartService.GetCartItems();
        if (!cartItems.Any())
        {
            Console.WriteLine("Koszyk jest pusty.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Produkty w koszyku:");
        for (int i = 0; i < cartItems.Count; i++)
        {
            var item = cartItems[i];
            Console.WriteLine($"{i + 1}. {item.Product.Name} x{item.Quantity} = {item.TotalPrice:C}");
        }

        Console.Write("Numer produktu do usunięcia: ");
        if (!int.TryParse(Console.ReadLine(), out var index) || index < 1 || index > cartItems.Count)
        {
            Console.WriteLine("Nieprawidłowy numer!");
            Console.ReadKey();
            return;
        }

        var removedItem = cartItems[index - 1];
        _cartService.RemoveItem(removedItem.ProductId);
        Console.WriteLine($"Usunięto '{removedItem.Product.Name}' z koszyka.");
        Console.ReadKey();
    }

    private async Task PlaceOrderAsync()
    {
        Console.Clear();
        
        var cartItems = _cartService.GetCartItems();
        if (!cartItems.Any())
        {
            Console.WriteLine("Koszyk jest pusty. Nie można złożyć zamówienia.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("=== SKŁADANIE ZAMÓWIENIA ===");
        Console.WriteLine("Zawartość zamówienia:");
        
        Console.WriteLine($"{"Produkt",-20} {"Ilość",-8} {"Wartość",-10}");
        Console.WriteLine(new string('-', 40));
        
        foreach (var item in cartItems)
        {
            Console.WriteLine($"{item.Product.Name,-20} {item.Quantity,-8} {item.TotalPrice,-10:C}");
        }
        
        Console.WriteLine(new string('-', 40));
        Console.WriteLine($"Łączna wartość: {_cartService.GetTotalAmount(),-20:C}");
        
        Console.WriteLine("\nDane klienta (opcjonalne):");
        Console.Write("Imię i nazwisko: ");
        var customerName = Console.ReadLine();
        
        Console.Write("Email: ");
        var customerEmail = Console.ReadLine();
        
        Console.WriteLine("\nCzy chcesz złożyć to zamówienie? (t/n)");
        
        var confirm = Console.ReadLine()?.ToLower();
        if (confirm != "t" && confirm != "tak")
        {
            Console.WriteLine("Zamówienie anulowane.");
            Console.ReadKey();
            return;
        }

        try
        {
            var orderItems = cartItems.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList();

            var order = await _orderController.CreateOrderAsync(new CreateOrderRequest
            {
                Items = orderItems,
                CustomerName = customerName,
                CustomerEmail = customerEmail
            });
            
            Console.WriteLine();
            Console.WriteLine("Zamówienie zostało złożone!");
            Console.WriteLine($"Numer zamówienia: {order.OrderNumber}");

            await GenerateReceiptAsync(order.Id);

            _cartService.ClearCart();

            Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas składania zamówienia: {ex.Message}");
            Console.ReadKey();
        }
    }

    private async Task GenerateReceiptAsync(int orderId)
    {
        try
        {
            var receipt = await _orderController.GenerateReceiptAsync(orderId);
            
            Console.WriteLine();
            Console.WriteLine("==========================================");
            Console.WriteLine("                PARAGON");
            Console.WriteLine("==========================================");
            Console.WriteLine($"Data: {receipt.CreatedAt:dd.MM.yyyy HH:mm}");
            Console.WriteLine($"Numer zamówienia: {receipt.OrderNumber}");
            
            if (!string.IsNullOrEmpty(receipt.CustomerName))
            {
                Console.WriteLine($"Klient: {receipt.CustomerName}");
            }
            
            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"{"Produkt",-20} {"Ilość",-8} {"Wartość",-10}");
            Console.WriteLine("------------------------------------------");
            
            foreach (var item in receipt.Items)
            {
                Console.WriteLine($"{item.ProductName,-20} x{item.Quantity,-2} {item.TotalPrice,8:C}");
            }
            
            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"SUMA: {receipt.TotalAmount,30:C}");
            Console.WriteLine("==========================================");
            Console.WriteLine("Dziękujemy za zakupy!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas generowania paragonu: {ex.Message}");
        }
    }
}
