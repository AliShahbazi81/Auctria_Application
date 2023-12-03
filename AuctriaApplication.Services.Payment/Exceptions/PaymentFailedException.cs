namespace AuctriaApplication.Services.Payment.Exceptions;

public class PaymentFailedException : Exception
{
    public PaymentFailedException() 
        : base("Sorry, but payment could not be completed.")
    {
    }

    public PaymentFailedException(string message) 
        : base(message)
    {
    }

    public PaymentFailedException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}