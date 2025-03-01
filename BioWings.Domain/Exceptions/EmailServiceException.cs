namespace BioWings.Domain.Exceptions;

public class EmailServiceException : Exception
{
    public EmailServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}