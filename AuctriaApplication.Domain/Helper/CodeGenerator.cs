namespace AuctriaApplication.Domain.Helper;

public static class CodeGenerator
{
    public static string Generate()
    {
        var generator = new Random();
        var verificationCode = generator.Next(0, 1_000_000).ToString("D6");

        return verificationCode;
    }
}