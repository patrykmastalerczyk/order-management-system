using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Interfaces;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Presentation.Controllers;

namespace OrderManagementSystem.Presentation.Views;

public class SellerDashboard
{
    private readonly AuthenticationController _authController;
    private readonly ProductController _productController;
    private readonly OrderController _orderController;

    public SellerDashboard(
        AuthenticationController authController,
        ProductController productController,
        OrderController orderController)
    {
        _authController = authController;
        _productController = productController;
        _orderController = orderController;
    }

    public async Task DisplayAsync()
    {
        while (true)
        {
            Console.Clear();
            var currentUser = _authController.GetCurrentUser();
            
            Console.WriteLine($"=== PANEL SPRZEDAWCY ===");
            Console.WriteLine($"Zalogowany jako: {currentUser?.FullName} ({currentUser?.Role})");
            Console.WriteLine("1. Zarządzaj produktami");
            Console.WriteLine("2. Przeglądaj zamówienia");
            Console.WriteLine("3. Zarządzaj statusami zamówień");
            Console.WriteLine("4. Raporty");
            Console.WriteLine("5. Wyloguj się");
            Console.WriteLine("========================");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ManageProductsAsync();
                    break;
                case "2":
                    await ViewOrdersAsync();
                    break;
                case "3":
                    await ManageOrderStatusesAsync();
                    break;
                case "4":
                    await ShowReportsAsync();
                    break;
                case "5":
                    _authController.Logout();
                    return;
                default:
                    Console.WriteLine("Nieprawidłowy wybór.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private async Task ManageProductsAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== ZARZĄDZANIE PRODUKTAMI ===");
            Console.WriteLine("1. Wyświetl wszystkie produkty");
            Console.WriteLine("2. Dodaj nowy produkt");
            Console.WriteLine("3. Edytuj produkt");
            Console.WriteLine("4. Usuń produkt");
            Console.WriteLine("5. Wróć");
            Console.WriteLine("==============================");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await DisplayProductsAsync();
                    break;
                case "2":
                    await AddProductAsync();
                    break;
                case "3":
                    await EditProductAsync();
                    break;
                case "4":
                    await DeleteProductAsync();
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

    private async Task DisplayProductsAsync()
    {
        Console.Clear();
        Console.WriteLine("=== LISTA PRODUKTÓW ===");
        
        var products = await _productController.GetAllProductsAsync();
        
        if (!products.Any())
        {
            Console.WriteLine("Brak produktów w bazie.");
        }
        else
        {
            Console.WriteLine($"{"ID",-4} {"Nazwa",-20} {"Opis",-30} {"Cena",-10} {"Stan",-8} {"Status"}");
            Console.WriteLine(new string('-', 80));
            
            foreach (var product in products)
            {
                Console.WriteLine($"{product.Id,-4} {product.Name,-20} {product.Description,-30} {product.Price,-10:C} {product.StockQuantity,-8} {(product.IsActive ? "Aktywny" : "Nieaktywny")}");
            }
        }
        
        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    private async Task AddProductAsync()
    {
        Console.Clear();
        Console.WriteLine("=== DODAWANIE NOWEGO PRODUKTU ===");
        
        Console.Write("Nazwa produktu: ");
        var name = Console.ReadLine();
        
        Console.Write("Opis produktu: ");
        var description = Console.ReadLine();
        
        Console.Write("Cena: ");
        if (!decimal.TryParse(Console.ReadLine(), out var price) || price < 0)
        {
            Console.WriteLine("Nieprawidłowa cena!");
            Console.ReadKey();
            return;
        }

        Console.Write("Stan magazynowy: ");
        if (!int.TryParse(Console.ReadLine(), out var stock) || stock < 0)
        {
            Console.WriteLine("Nieprawidłowy stan magazynowy!");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\nKategorie:");
        Console.WriteLine("1. Kawa (Coffee)");
        Console.WriteLine("2. Bajgle (Bagels)");
        Console.WriteLine("3. Rogale (Croissants)");
        Console.WriteLine("4. Ciasta (Cakes)");
        Console.Write("Wybierz kategorię (1-4): ");
        
        if (!int.TryParse(Console.ReadLine(), out var categoryChoice) || categoryChoice < 1 || categoryChoice > 4)
        {
            Console.WriteLine("Nieprawidłowa kategoria!");
            Console.ReadKey();
            return;
        }

        var category = categoryChoice switch
        {
            1 => ProductCategory.Coffee,
            2 => ProductCategory.Bagels,
            3 => ProductCategory.Croissants,
            4 => ProductCategory.Cakes,
            _ => ProductCategory.Coffee
        };

        try
        {
            var product = await _productController.CreateProductAsync(new CreateProductRequest
            {
                Name = name ?? "",
                Description = description ?? "",
                Price = price,
                StockQuantity = stock,
                Category = category
            });
            
            Console.WriteLine($"Produkt '{product.Name}' został dodany pomyślnie!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas dodawania produktu: {ex.Message}");
        }
        
        Console.ReadKey();
    }

    private async Task EditProductAsync()
    {
        Console.Clear();
        Console.WriteLine("=== EDYCJA PRODUKTU ===");
        
        Console.Write("ID produktu do edycji: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("Nieprawidłowe ID!");
            Console.ReadKey();
            return;
        }

        var product = await _productController.GetProductByIdAsync(id);
        if (product == null)
        {
            Console.WriteLine("Produkt o podanym ID nie istnieje!");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"Aktualne dane: {product.Name} | {product.Description} | {product.Price:C} | Stan: {product.StockQuantity} | Kategoria: {product.Category}");
        
        Console.Write("Nowa nazwa (Enter = bez zmian): ");
        var newName = Console.ReadLine();
        
        Console.Write("Nowy opis (Enter = bez zmian): ");
        var newDescription = Console.ReadLine();
        
        Console.Write("Nowa cena (Enter = bez zmian): ");
        var priceInput = Console.ReadLine();
        
        Console.Write("Nowy stan magazynowy (Enter = bez zmian): ");
        var stockInput = Console.ReadLine();

        Console.WriteLine("\nKategorie:");
        Console.WriteLine("1. Kawa (Coffee)");
        Console.WriteLine("2. Bajgle (Bagels)");
        Console.WriteLine("3. Rogale (Croissants)");
        Console.WriteLine("4. Ciasta (Cakes)");
        Console.Write("Nowa kategoria (Enter = bez zmian, 1-4): ");
        var categoryInput = Console.ReadLine();
        
        ProductCategory newCategory;
        if (!string.IsNullOrWhiteSpace(categoryInput) && int.TryParse(categoryInput, out var categoryChoice) && categoryChoice >= 1 && categoryChoice <= 4)
        {
            newCategory = categoryChoice switch
            {
                1 => ProductCategory.Coffee,
                2 => ProductCategory.Bagels,
                3 => ProductCategory.Croissants,
                4 => ProductCategory.Cakes,
                _ => Enum.TryParse<ProductCategory>(product.Category, out var existing) ? existing : ProductCategory.Coffee
            };
        }
        else
        {
            // Jeśli użytkownik nie podał nowej kategorii, użyj istniejącej
            newCategory = Enum.TryParse<ProductCategory>(product.Category, out var existing) ? existing : ProductCategory.Coffee;
        }

        var updateRequest = new UpdateProductRequest
        {
            Id = id,
            Name = string.IsNullOrWhiteSpace(newName) ? product.Name : newName,
            Description = string.IsNullOrWhiteSpace(newDescription) ? product.Description : newDescription,
            Price = string.IsNullOrWhiteSpace(priceInput) ? product.Price : (decimal.TryParse(priceInput, out var newPrice) ? newPrice : product.Price),
            StockQuantity = string.IsNullOrWhiteSpace(stockInput) ? product.StockQuantity : (int.TryParse(stockInput, out var newStock) ? newStock : product.StockQuantity),
            Category = newCategory
        };

        var success = await _productController.UpdateProductAsync(updateRequest);
        Console.WriteLine(success ? "Produkt został zaktualizowany!" : "Nie udało się zaktualizować produktu!");
        Console.ReadKey();
    }

    private async Task DeleteProductAsync()
    {
        Console.Clear();
        Console.WriteLine("=== USUWANIE PRODUKTU ===");
        
        Console.Write("ID produktu do usunięcia: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("Nieprawidłowe ID!");
            Console.ReadKey();
            return;
        }

        var product = await _productController.GetProductByIdAsync(id);
        if (product == null)
        {
            Console.WriteLine("Produkt o podanym ID nie istnieje!");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"Czy na pewno chcesz usunąć produkt '{product.Name}'? (t/n)");
        var confirm = Console.ReadLine()?.ToLower();
        
        if (confirm == "t" || confirm == "tak")
        {
            var success = await _productController.DeleteProductAsync(id);
            Console.WriteLine(success ? "Produkt został usunięty!" : "Nie udało się usunąć produktu!");
        }
        else
        {
            Console.WriteLine("Operacja anulowana.");
        }
        
        Console.ReadKey();
    }

    private async Task ViewOrdersAsync()
    {
        Console.Clear();
        Console.WriteLine("=== HISTORIA ZAMÓWIEŃ ===");
        
        var orders = await _orderController.GetAllOrdersAsync();
        
        if (!orders.Any())
        {
            Console.WriteLine("Brak zamówień w systemie.");
        }
        else
        {
            foreach (var order in orders.Take(10)) // Pokaż ostatnie 10 zamówień
            {
                Console.WriteLine($"Zamówienie: {order.OrderNumber}");
                Console.WriteLine($"Status: {order.Status} | Data: {order.CreatedAt:dd.MM.yyyy HH:mm}");
                Console.WriteLine($"Wartość: {order.TotalAmount:C}");
                if (!string.IsNullOrEmpty(order.CustomerName))
                {
                    Console.WriteLine($"Klient: {order.CustomerName}");
                }
                Console.WriteLine("Produkty:");
                
                foreach (var item in order.Items)
                {
                    Console.WriteLine($"  - {item.ProductName} x{item.Quantity} = {item.TotalPrice:C}");
                }
                Console.WriteLine("---");
            }
        }
        
        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    private async Task ManageOrderStatusesAsync()
    {
        Console.Clear();
        Console.WriteLine("=== ZARZĄDZANIE STATUSAMI ZAMÓWIEŃ ===");
        
        var orders = await _orderController.GetAllOrdersAsync();
        if (!orders.Any())
        {
            Console.WriteLine("Brak zamówień w systemie.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Dostępne zamówienia:");
        foreach (var order in orders.Take(20))
        {
            Console.WriteLine($"ID: {order.Id} | {order.OrderNumber} | Status: {order.Status} | {order.TotalAmount:C}");
        }

        Console.Write("ID zamówienia: ");
        if (!int.TryParse(Console.ReadLine(), out var orderId))
        {
            Console.WriteLine("Nieprawidłowe ID!");
            Console.ReadKey();
            return;
        }

        var selectedOrder = await _orderController.GetOrderByIdAsync(orderId);
        if (selectedOrder == null)
        {
            Console.WriteLine("Zamówienie o podanym ID nie istnieje!");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"Aktualny status: {selectedOrder.Status}");
        Console.WriteLine("Nowy status:");
        Console.WriteLine("1. Pending (Oczekujące)");
        Console.WriteLine("2. InProgress (W trakcie)");
        Console.WriteLine("3. Completed (Zakończone)");
        Console.WriteLine("4. Cancelled (Anulowane)");
        Console.WriteLine("5. Shipped (Wysłane)");

        Console.Write("Wybierz status (1-5): ");
        if (!int.TryParse(Console.ReadLine(), out var statusChoice) || statusChoice < 1 || statusChoice > 5)
        {
            Console.WriteLine("Nieprawidłowy wybór!");
            Console.ReadKey();
            return;
        }

        var statusNames = new[] { "Pending", "InProgress", "Completed", "Cancelled", "Shipped" };
        var newStatus = statusNames[statusChoice - 1];

        var success = await _orderController.UpdateOrderStatusAsync(new UpdateOrderStatusRequest
        {
            OrderId = orderId,
            Status = newStatus
        });

        Console.WriteLine(success 
            ? $"Status zamówienia {selectedOrder.OrderNumber} został zmieniony na: {newStatus}"
            : "Nie udało się zmienić statusu zamówienia!");
        
        Console.ReadKey();
    }

    private async Task ShowReportsAsync()
    {
        Console.Clear();
        Console.WriteLine("=== RAPORTY ===");
        
        var orders = await _orderController.GetAllOrdersAsync();
        var products = await _productController.GetAllProductsAsync();
        
        Console.WriteLine($"Łączna liczba zamówień: {orders.Count}");
        Console.WriteLine($"Łączna wartość zamówień: {orders.Sum(o => o.TotalAmount):C}");
        Console.WriteLine($"Liczba produktów w systemie: {products.Count}");
        Console.WriteLine($"Produkty na stanie: {products.Sum(p => p.StockQuantity)}");
        
        Console.WriteLine("\nZamówienia według statusu:");
        var statusGroups = orders.GroupBy(o => o.Status);
        foreach (var group in statusGroups)
        {
            Console.WriteLine($"  {group.Key}: {group.Count()} zamówień");
        }
        
        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }
}
