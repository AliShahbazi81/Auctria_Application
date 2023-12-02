using System.Text;

namespace AuctriaApplication.Domain.Helper;

public static class StringHelper
{
    public static string AddSpacesToCamelCase(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var newText = new StringBuilder(input[0].ToString());

        for (var i = 1; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
                newText.Append(' ');

            newText.Append(input[i]);
        }

        return newText.ToString();
    }
    
    public static string ConvertFirstLetterToUpper(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        input = input.ToLower();
        var words = input.Split(' ');

        for (var i = 0; i < words.Length; i++)
            if (words[i].Length > 0)
                words[i] = char.ToUpper(words[i][0]) + words[i][1..];

        return string.Join(" ", words);
    }

    public static string? ConvertToLowerCaseNoSpaces(string? input)
    {
        return input?.ToLowerInvariant().Replace(" ", "");
    }
}