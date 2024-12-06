namespace BioWings.Domain.Exceptions;

public class NullEntityException(string entityName) : Exception($"Entity '{entityName}' cannot be null.")
{
}
