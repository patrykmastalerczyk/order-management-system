namespace OrderManagementSystem;

class Program
{
    static async Task Main(string[] args)
    {
        var app = new OrderManagementApplication();
        await app.RunAsync();
    }
}