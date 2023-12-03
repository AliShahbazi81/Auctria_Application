namespace AuctriaApplication.Infrastructure.Payment.Guard;

public static class PaymentGuard
{
    public static bool IsCardNumberValid(string cardNumber)
    {
        return cardNumber.Length == 16;
    }

    public static bool IsCvvValid(string cvv)
    {
        return cvv.Length == 3;
    }
    
    public static bool IsCardExpired(string month, string year)
    {
        if (!int.TryParse(month, out var expMonth) || !int.TryParse(year, out var expYear))
            return true; 

        var lastDayOfExpMonth = DateTime.DaysInMonth(expYear, expMonth);
        var expDate = new DateTime(expYear, expMonth, lastDayOfExpMonth);

        // Check if the expiration date is before today's date
        return expDate < DateTime.Today.ToUniversalTime();
    }
}