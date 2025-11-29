using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Core.Interfaces;
using OrderManagementSystem.Core.Services;
using OrderManagementSystem.Infrastructure.Authentication;
using OrderManagementSystem.Infrastructure.Data;
using OrderManagementSystem.Presentation.Controllers;
using OrderManagementSystem.Presentation.Views;

namespace OrderManagementSystem;

public class OrderManagementApplication
{
    private readonly AuthenticationController _authController;
    private readonly ProductController _productController;
    private readonly OrderController _orderController;
    private readonly SellerDashboard _sellerDashboard;
    private readonly CustomerShoppingView _customerView;
    private readonly ApplicationDbContext _context;

    public OrderManagementApplication()
    {
        try
        {
            var dir = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
            var dbPath = Path.Combine(dir, "order-management-db.sqlite");

            Console.WriteLine("=== INICJALIZACJA BAZY DANYCH ===");
            Console.WriteLine($"Lokalizacja bazy danych: {dbPath}");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            _context = new ApplicationDbContext(optionsBuilder.Options);

            var seeder = new DatabaseSeeder(_context);
            seeder.SeedAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ KRYTYCZNY BŁĄD podczas inicjalizacji bazy danych!");
            Console.WriteLine($"Błąd: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Wewnętrzny błąd: {ex.InnerException.Message}");
            }
            Console.WriteLine("\nNaciśnij dowolny klawisz, aby wyjść...");
            Console.ReadKey();
            throw;
        }

        var userRepository = new UserRepository(_context);
        var productRepository = new ProductRepository(_context);
        var orderRepository = new OrderRepository(_context);

        var authService = new AuthenticationService(userRepository);
        var productService = new ProductManagementService(productRepository);
        var orderService = new OrderManagementService(orderRepository, productRepository);
        var cartService = new ShoppingCartService();

        _authController = new AuthenticationController(authService);
        _productController = new ProductController(productService);
        _orderController = new OrderController(orderService);

        _sellerDashboard = new SellerDashboard(_authController, _productController, _orderController);
        _customerView = new CustomerShoppingView(_productController, _orderController, cartService);
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Witamy w Systemie Zarządzania Zamówieniami!");
        Console.WriteLine("==========================================");

        while (true)
        {
            Console.Clear();
            
            Console.WriteLine("=== SYSTEM ZAMÓWIEŃ ===");
            Console.WriteLine("1. Zaloguj się jako sprzedawca");
            Console.WriteLine("2. Sklep (klient)");
            Console.WriteLine("3. Wyjdź");
            Console.WriteLine("======================");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await LoginSellerAsync();
                    break;
                case "2":
                    await _customerView.DisplayAsync();
                    break;
                case "3":
                    Console.WriteLine("Dziękujemy za korzystanie z systemu!");
                    return;
                default:
                    Console.WriteLine("Nieprawidłowy wybór.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private async Task LoginSellerAsync()
    {
        Console.Clear();
        Console.WriteLine("=== LOGOWANIE SPRZEDAWCY ===");
        
        Console.Write("Nazwa użytkownika: ");
        var username = Console.ReadLine();
        
        Console.Write("Hasło: ");
        var password = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Nazwa użytkownika i hasło nie mogą być puste!");
            Console.ReadKey();
            return;
        }

        var loginRequest = new DTOs.LoginRequest
        {
            Username = username,
            Password = password
        };

        var response = await _authController.LoginAsync(loginRequest);
        
        if (response.Success)
        {
            Console.WriteLine($"Witaj, {response.User?.FullName}!");
            Console.WriteLine("Naciśnij dowolny klawisz, aby przejść do panelu sprzedawcy...");
            Console.ReadKey();
            
            await _sellerDashboard.DisplayAsync();
        }
        else
        {
            Console.WriteLine(response.Message);
            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
        }
    }
}
