namespace AuctriaApplication.Services.Store.Exceptions;

public class NotSavedException : Exception
{
    public NotSavedException(bool isCreating) : base($"Sorry, but there is a problem {(isCreating ? "creating" : "updating")}")
    {
        
    }
    public NotSavedException(string? categoryName) : base($"Sorry, but there is a problem saving the category {categoryName}")
    {
        
    }
}