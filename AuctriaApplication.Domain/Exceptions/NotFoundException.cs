namespace AuctriaApplication.Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException() : base("Sorry, but we could not find the item you are looking for.")
    {
        
    }
}