namespace BioWings.Domain.Exceptions;
public class NotFoundException(int id) : Exception($"Id : ({id}) was not found.")
{
}
