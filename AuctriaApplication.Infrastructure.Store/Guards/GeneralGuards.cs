namespace AuctriaApplication.Infrastructure.Store.Guards;

public static class GeneralGuards
{
    public static bool AreInputsNull(Guid? id, string? name)
    {
        return id is not null || !string.IsNullOrWhiteSpace(name);
    }
    
    public static bool IsNumberMoreThanZero(int number)
    {
        return number >= 0;
    }
}