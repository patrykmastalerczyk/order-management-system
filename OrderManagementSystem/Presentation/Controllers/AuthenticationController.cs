using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Interfaces;
using OrderManagementSystem.DTOs;

namespace OrderManagementSystem.Presentation.Controllers;

public class AuthenticationController
{
    private readonly IAuthenticationService _authService;

    public AuthenticationController(IAuthenticationService authService)
    {
        _authService = authService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request.Username, request.Password);
            
            if (result.IsSuccess && result.User != null)
            {
                return new LoginResponse
                {
                    Success = true,
                    Message = "Logowanie pomyślne",
                    User = new UserDto
                    {
                        Id = result.User.Id,
                        Username = result.User.Username,
                        FullName = result.User.FullName,
                        Role = result.User.Role.ToString(),
                        CreatedAt = result.User.CreatedAt,
                        IsActive = result.User.IsActive
                    }
                };
            }

            return new LoginResponse
            {
                Success = false,
                Message = result.ErrorMessage ?? "Nieprawidłowe dane logowania"
            };
        }
        catch (Exception ex)
        {
            return new LoginResponse
            {
                Success = false,
                Message = $"Błąd logowania: {ex.Message}"
            };
        }
    }

    public void Logout()
    {
        _authService.Logout();
    }

    public UserDto? GetCurrentUser()
    {
        var user = _authService.GetCurrentUser();
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt,
            IsActive = user.IsActive
        };
    }

    public bool IsAuthenticated()
    {
        return _authService.IsAuthenticated();
    }
}
