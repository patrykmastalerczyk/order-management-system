namespace OrderManagementSystem.Core.Entities;

public class User
{
    public int Id { get; private set; }
    public string Username { get; private set; }
    public string Password { get; private set; }
    public string FullName { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    private User() 
    {
        Username = string.Empty;
        Password = string.Empty;
        FullName = string.Empty;
    }

    public User(string username, string password, string fullName, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty", nameof(username));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be null or empty", nameof(fullName));

        Username = username;
        Password = password;
        FullName = fullName;
        Role = role;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void UpdateProfile(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be null or empty", nameof(fullName));
        
        FullName = fullName;
    }

    public void ChangePassword(string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("Password cannot be null or empty", nameof(newPassword));
        
        Password = newPassword;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public bool HasRole(UserRole requiredRole)
    {
        return Role >= requiredRole;
    }

    public static User CreateWithId(int id, string username, string password, string fullName, UserRole role)
    {
        var user = new User(username, password, fullName, role);
        var idProperty = typeof(User).GetProperty("Id");
        idProperty?.SetValue(user, id);
        return user;
    }
}

public enum UserRole
{
    Seller = 0,
    Manager = 1,
    Admin = 2
}
