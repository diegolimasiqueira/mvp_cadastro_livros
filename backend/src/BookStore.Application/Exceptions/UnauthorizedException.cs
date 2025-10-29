namespace BookStore.Application.Exceptions;

/// <summary>
/// Exceção lançada quando há falha de autenticação
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException() : base("Unauthorized access")
    {
    }
}



