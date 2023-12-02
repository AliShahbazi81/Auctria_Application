namespace AuctriaApplication.Infrastructure.Membership.Guards;

public static class UserManagerGuard
{
    public static bool IsCodeValid(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;
        return code.Length == 6;
    }
}