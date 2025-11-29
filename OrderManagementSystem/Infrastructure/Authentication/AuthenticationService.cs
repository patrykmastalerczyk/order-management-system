using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Interfaces;

namespace OrderManagementSystem.Infrastructure.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private User? _currentUser;

    public AuthenticationService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<AuthenticationResult> LoginAsync(string username, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username))
                return AuthenticationResult.Failure("Username cannot be empty");

            if (string.IsNullOrWhiteSpace(password))
                return AuthenticationResult.Failure("Password cannot be empty");

            var user = await _userRepository.GetByUsernameAndPasswordAsync(username, password);
            
            if (user == null)
                return AuthenticationResult.Failure("Invalid username or password");

            if (!user.IsActive)
                return AuthenticationResult.Failure("User account is deactivated");

            _currentUser = user;
            return AuthenticationResult.Success(user);
        }
        catch (Exception ex)
        {
            return AuthenticationResult.Failure($"Authentication failed: {ex.Message}");
        }
    }

    public void Logout()
    {
        _currentUser = null;
    }

    public User? GetCurrentUser()
    {
        return _currentUser;
    }

    public bool IsAuthenticated()
    {
        return _currentUser != null && _currentUser.IsActive;
    }

    public async Task<bool> ValidateCredentialsAsync(string username, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            var user = await _userRepository.GetByUsernameAndPasswordAsync(username, password);
            return user != null && user.IsActive;
        }
        catch
        {
            return false;
        }
    }
}
