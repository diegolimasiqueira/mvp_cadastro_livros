using BookStore.Application.DTOs.Auth;

namespace BookStore.Application.Interfaces;

/// <summary>
/// Interface para serviço de autenticação
/// </summary>
public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
}



