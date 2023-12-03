namespace AuctriaApplication.Services.Store.Exceptions;

public class NotEnoughQuantityException : Exception
{
    public NotEnoughQuantityException(string productName) : base($"Sorry, there is not enough quantity of {productName} to fulfill your order.")
    {
        
    }
}