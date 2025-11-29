using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Core.Entities;

namespace OrderManagementSystem.Infrastructure.Data;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;

    public DatabaseSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        try
        {
            var created = await _context.Database.EnsureCreatedAsync();
            if (created)
            {
                Console.WriteLine("Baza danych SQLite została utworzona pomyślnie.");
            }
            else
            {
                Console.WriteLine("Baza danych SQLite już istnieje.");
            }

            if (!await _context.Users.AnyAsync())
            {
                var users = new[]
                {
                    User.CreateWithId(1, "admin", "admin", "Administrator", UserRole.Admin),
                };

                await _context.Users.AddRangeAsync(users);
                Console.WriteLine($"Dodano {users.Length} użytkowników do bazy danych.");
            }
            else
            {
                var userCount = await _context.Users.CountAsync();
                Console.WriteLine($"Baza danych zawiera już {userCount} użytkowników.");
            }

            if (!await _context.Products.AnyAsync())
            {
                var products = new[]
                {
                    // Kawa (Coffee)
                    Product.CreateWithId(1, "Espresso", "Mocna, aromatyczna kawa espresso", 8.50m, 50, ProductCategory.Coffee),
                    Product.CreateWithId(2, "Cappuccino", "Kawa espresso z mlekiem i pianką", 12.00m, 45, ProductCategory.Coffee),
                    Product.CreateWithId(3, "Latte", "Delikatna kawa z dużą ilością mleka", 13.50m, 40, ProductCategory.Coffee),
                    Product.CreateWithId(4, "Americano", "Espresso rozcieńczone gorącą wodą", 10.00m, 35, ProductCategory.Coffee),
                    Product.CreateWithId(5, "Macchiato", "Espresso z kroplą mleka", 9.50m, 30, ProductCategory.Coffee),
                    
                    // Bajgle (Bagels)
                    Product.CreateWithId(6, "Bajgiel klasyczny", "Tradycyjny bajgiel z kremowym serkiem", 6.50m, 60, ProductCategory.Bagels),
                    Product.CreateWithId(7, "Bajgiel z sezamem", "Bajgiel posypany sezamem", 7.00m, 55, ProductCategory.Bagels),
                    Product.CreateWithId(8, "Bajgiel z makiem", "Bajgiel z makiem i kremem", 7.50m, 50, ProductCategory.Bagels),
                    Product.CreateWithId(9, "Bajgiel pełnoziarnisty", "Zdrowy bajgiel pełnoziarnisty", 8.00m, 45, ProductCategory.Bagels),
                    
                    // Rogale (Croissants)
                    Product.CreateWithId(10, "Rogal maślany", "Delikatny, maślany rogal francuski", 9.00m, 40, ProductCategory.Croissants),
                    Product.CreateWithId(11, "Rogal czekoladowy", "Rogal z nadzieniem czekoladowym", 10.50m, 35, ProductCategory.Croissants),
                    Product.CreateWithId(12, "Rogal migdałowy", "Rogal z kremem migdałowym", 11.00m, 30, ProductCategory.Croissants),
                    
                    // Ciasta (Cakes)
                    Product.CreateWithId(13, "Ciasto czekoladowe", "Mocne ciasto czekoladowe z bitą śmietaną", 15.00m, 25, ProductCategory.Cakes),
                    Product.CreateWithId(14, "Ciasto marchewkowe", "Ciepłe ciasto marchewkowe z kremem", 14.50m, 20, ProductCategory.Cakes),
                    Product.CreateWithId(15, "Ciasto serowe", "Klasyczne ciasto serowe z owocami", 16.00m, 22, ProductCategory.Cakes),
                    Product.CreateWithId(16, "Ciasto waniliowe", "Delikatne ciasto waniliowe z polewą", 14.00m, 18, ProductCategory.Cakes)
                };

                await _context.Products.AddRangeAsync(products);
                Console.WriteLine($"Dodano {products.Length} produktów do bazy danych (kawiarnia).");
                Console.WriteLine($"  - Kawa: 5 produktów");
                Console.WriteLine($"  - Bajgle: 4 produkty");
                Console.WriteLine($"  - Rogale: 3 produkty");
                Console.WriteLine($"  - Ciasta: 4 produkty");
            }
            else
            {
                var productCount = await _context.Products.CountAsync();
                Console.WriteLine($"Baza danych zawiera już {productCount} produktów.");
            }

            await _context.SaveChangesAsync();
            Console.WriteLine("Inicjalizacja bazy danych zakończona pomyślnie.\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas inicjalizacji bazy danych: {ex.Message}");
            Console.WriteLine($"Szczegóły: {ex}");
            throw;
        }
    }
}
