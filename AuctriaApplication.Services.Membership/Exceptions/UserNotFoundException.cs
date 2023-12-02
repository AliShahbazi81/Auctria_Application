namespace AuctriaApplication.Services.Membership.Exceptions;

public class UserNotFoundException : Exception
{
    public string Username { get; }

    public UserNotFoundException(string username) : base($"Username {username} not found")
    {
        Username = username;
    }

    public UserNotFoundException() : base("User not found")
    {
    }
}