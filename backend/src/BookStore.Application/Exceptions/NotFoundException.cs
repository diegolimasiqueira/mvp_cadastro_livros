namespace BookStore.Application.Exceptions;

/// <summary>
/// Exceção lançada quando um recurso não é encontrado
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entity, int id) 
        : base($"{entity} with id {id} was not found")
    {
    }
}



