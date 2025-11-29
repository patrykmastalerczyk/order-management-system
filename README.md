# System Zarządzania Zamówieniami

## Architektura Aplikacji

Aplikacja napisana zgodnie z zasadami **Clean Architecture** i **Domain-Driven Design**:

```
OrderManagementSystem/
├── Core/                           # Logika biznesowa
│   ├── Entities/                  # Encje domenowe
│   │   ├── User.cs               # Użytkownik z rolami
│   │   ├── Product.cs            # Produkt z opisem i stanem
│   │   └── Order.cs              # Zamówienie z elementami
│   ├── Interfaces/               # Interfejsy dla dependency injection
│   │   ├── IRepositories.cs     # Interfejsy repozytoriów
│   │   └── IServices.cs         # Interfejsy serwisów biznesowych
│   └── Services/                 # Serwisy biznesowe
│       ├── ProductManagementService.cs
│       ├── OrderManagementService.cs
│       └── ShoppingCartService.cs
├── Infrastructure/               # Warstwa infrastruktury
│   ├── Data/                    # Implementacje repozytoriów
│   │   ├── UserRepository.cs
│   │   ├── ProductRepository.cs
│   │   └── OrderRepository.cs
│   └── Authentication/          # Logika uwierzytelniania
│       └── AuthenticationService.cs
├── Presentation/                 # Warstwa prezentacji
│   ├── Controllers/             # Kontrolery (MVC pattern)
│   │   ├── AuthenticationController.cs
│   │   ├── ProductController.cs
│   │   └── OrderController.cs
│   └── Views/                   # Widoki użytkownika
│       ├── SellerDashboard.cs
│       └── CustomerShoppingView.cs
├── DTOs/                        # Obiekty transferu danych
│   └── CommonDTOs.cs
├── OrderManagementApplication.cs # Główna aplikacja
└── Program.cs                   # Punkt wejścia
```

## Kluczowe Ulepszenia

### 🏗️ **Architektura**
- **Separation of Concerns** - każda warstwa ma określoną odpowiedzialność
- **Dependency Injection** - luźne powiązania między komponentami
- **Interface Segregation** - małe, skupione interfejsy
- **Async/Await** - asynchroniczne operacje dla lepszej wydajności

### 📊 **Rozszerzone Modele Danych**
- **User** z rolami (Admin, Manager, Seller)
- **Product** z opisem, stanem magazynowym i statusem aktywności
- **Order** z elementami, statusami i danymi klienta
- **OrderItem** jako osobna encja z ceną jednostkową

### 🔐 **Ulepszone Uwierzytelnianie**
- Różne role użytkowników
- Bezpieczne zarządzanie sesją
- Walidacja danych wejściowych

### 🛒 **Zaawansowany Koszyk Zakupów**
- Dynamiczne dodawanie/usuwanie produktów
- Aktualizacja ilości
- Walidacja dostępności w magazynie
- Automatyczne czyszczenie po złożeniu zamówienia

### 📋 **Rozszerzone Zarządzanie Zamówieniami**
- 5 statusów zamówień (Pending, InProgress, Completed, Cancelled, Shipped)
- Automatyczne zarządzanie stanem magazynowym
- Raporty i statystyki
- Filtrowanie według dat i statusów

## Funkcjonalności

### Dla Sprzedawców (po zalogowaniu):
1. **Zarządzanie produktami:**
   - Wyświetlanie wszystkich produktów z opisami
   - Dodawanie nowych produktów z opisem i stanem magazynowym
   - Edycja nazwy, opisu, ceny i stanu magazynowego
   - Usuwanie produktów (soft delete)

2. **Zarządzanie zamówieniami:**
   - Przeglądanie historii wszystkich zamówień
   - Filtrowanie według statusów
   - Zmiana statusu zamówień
   - Anulowanie zamówień z przywróceniem stanu magazynowego

3. **Raporty:**
   - Statystyki zamówień
   - Podsumowanie finansowe
   - Analiza według statusów

### Dla Klientów (bez logowania):
1. **Składanie zamówień:**
   - Przeglądanie dostępnych produktów z opisami
   - Dodawanie produktów do koszyka z określeniem ilości
   - Walidacja dostępności w magazynie
   - Wyświetlanie zawartości koszyka
   - Usuwanie produktów z koszyka
   - Składanie zamówienia z danymi klienta

2. **Paragon:**
   - Automatyczne generowanie paragonu po złożeniu zamówienia
   - Unikalny numer zamówienia
   - Szczegółowa lista produktów z cenami
   - Łączna wartość zamówienia
   - Dane klienta (opcjonalne)

## Dane Logowania

### Konta Testowe:
- **admin** / **admin** (Administrator)

## Statusy Zamówień

1. **Pending** - Oczekujące (nowo złożone)
2. **InProgress** - W trakcie realizacji
3. **Completed** - Zakończone
4. **Cancelled** - Anulowane
5. **Shipped** - Wysłane

## Uruchomienie

```bash
cd OrderManagementSystem/bin/Debug/net9.0/
./OrderManagementSystem
```

## Zalety Nowej Architektury

✅ **Modularność** - łatwe dodawanie nowych funkcji  
✅ **Testowalność** - możliwość mockowania zależności  
✅ **Skalowalność** - łatwe rozszerzanie o nowe warstwy  
✅ **Maintainability** - czytelny i zorganizowany kod  
✅ **Performance** - asynchroniczne operacje  
✅ **Security** - walidacja danych i bezpieczne uwierzytelnianie

## Najlepsze Praktyki Programowania Obiektowego

### 🏗️ **Encapsulation (Enkapsulacja)**
- **Private setters** - właściwości są chronione przed nieautoryzowanymi zmianami
- **Metody biznesowe** - logika biznesowa jest hermetyzowana w encjach
- **Walidacja** - wszystkie dane są walidowane przed zapisem
- **Immutable collections** - kolekcje są dostępne tylko do odczytu

### 🎯 **Single Responsibility Principle (SRP)**
- **Encje** - zawierają tylko logikę biznesową domeny
- **Serwisy** - obsługują konkretne przypadki użycia
- **Repozytoria** - zarządzają dostępem do danych
- **Kontrolery** - koordynują komunikację między warstwami

### 🔄 **Dependency Inversion Principle (DIP)**
- **Interfejsy** - abstrakcje nie zależą od szczegółów implementacji
- **Injection** - zależności są wstrzykiwane przez konstruktory
- **Loose coupling** - luźne powiązania między komponentami

### 📋 **Wzorce Projektowe**
- **Factory Pattern** - metody fabryczne dla tworzenia encji z ID
- **Result Pattern** - `ServiceResult<T>` i `AuthenticationResult` dla obsługi błędów
- **Repository Pattern** - abstrakcja dostępu do danych
- **Service Layer Pattern** - logika biznesowa w serwisach

### 🛡️ **Error Handling & Validation**
- **Walidacja danych** - `DataAnnotations` w DTOs
- **Obsługa wyjątków** - try-catch w serwisach
- **Result objects** - strukturalne zwracanie wyników operacji
- **Defensive programming** - sprawdzanie null i walidacja parametrów

### 🎨 **Clean Code Principles**
- **Meaningful names** - opisowe nazwy klas, metod i zmiennych
- **Small methods** - metody są krótkie i skupione na jednej odpowiedzialności
- **Consistent formatting** - spójne formatowanie kodu
- **Comments** - komentarze wyjaśniające złożoną logikę biznesową

### 🔒 **Security & Data Integrity**
- **Input validation** - walidacja wszystkich danych wejściowych
- **Business rules** - reguły biznesowe są egzekwowane w encjach
- **State management** - kontrola stanu obiektów przez metody biznesowe
- **Transaction safety** - przygotowanie do transakcji bazodanowych

### 📊 **Performance Optimizations**
- **Async/await** - asynchroniczne operacje dla lepszej wydajności
- **ReadOnly collections** - niezmienne kolekcje dla bezpieczeństwa
- **Lazy loading** - przygotowanie do leniwego ładowania danych
- **Memory efficiency** - optymalne zarządzanie pamięcią

## Przykłady Czystego Kodu

### Encja z hermetyzacją:
```csharp
public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    
    public Product(string name, string description, decimal price, int stockQuantity)
    {
        // Walidacja w konstruktorze
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be null or empty");
        
        Name = name;
        // ...
    }
    
    public void UpdateDetails(string name, string description, decimal price)
    {
        // Logika biznesowa hermetyzowana w encji
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be null or empty");
        
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

### Serwis z obsługą błędów:
```csharp
public async Task<ServiceResult<Product>> CreateProductAsync(CreateProductRequest request)
{
    try
    {
        var product = new Product(request.Name, request.Description, request.Price, request.StockQuantity);
        var createdProduct = await _productRepository.CreateAsync(product);
        return ServiceResult<Product>.Success(createdProduct);
    }
    catch (Exception ex)
    {
        return ServiceResult<Product>.Failure($"Failed to create product: {ex.Message}", ex);
    }
}
```

### Walidacja w DTOs:
```csharp
public class CreateProductRequest
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99")]
    public decimal Price { get; set; }
}
```
