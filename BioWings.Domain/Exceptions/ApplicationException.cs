namespace BioWings.Domain.Exceptions;

public class ApplicationException(string message) : Exception(message)
{
}