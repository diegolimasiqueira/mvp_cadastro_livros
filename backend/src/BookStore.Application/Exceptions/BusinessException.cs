namespace BookStore.Application.Exceptions;

/// <summary>
/// Exceção base para erros de regras de negócio
/// </summary>
public class BusinessException : Exception
{
    public BusinessException(string message) : base(message)
    {
    }

    public BusinessException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}



