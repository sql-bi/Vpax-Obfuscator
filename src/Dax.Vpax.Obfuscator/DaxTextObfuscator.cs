using System.Diagnostics;

namespace Dax.Vpax.Obfuscator;

internal sealed class DaxTextObfuscator
{
    // See DAX syntax https://learn.microsoft.com/en-us/dax/dax-syntax-reference
    internal const string AlphaCharSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    internal const string CharSet = /**/ "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; // ANSI alphanumeric character range
    internal const int RetryLimitBeforeExtension = 26; // A..Z

    /// <summary>
    /// A salt combined with the plaintext hashcode to ensure that a different obfuscated string is generated for each program execution
    /// </summary>
    private readonly int _instanceSalt;
    private readonly Random _random;

    public DaxTextObfuscator()
    {
        _random = new Random();
        _instanceSalt = _random.Next();
    }

    public DaxText Obfuscate(DaxText text, int retryCount = 0)
    {
        if (text.ObfuscatedValue != null && retryCount == 0) throw new InvalidOperationException("Text has already been obfuscated.");
        if (text.ObfuscatedValue == null && retryCount != 0) throw new InvalidOperationException("Text has not been obfuscated yet.");

        // Skip obfuscation for reserved tokens and empty or whitespace strings
        if (ReservedTokens.Contains(text.Value) || string.IsNullOrWhiteSpace(text.Value))
        {
            text.ObfuscatedValue = text.Value;
            return text;
        }

        var plaintext = text.Value;
        var salt = retryCount != 0 ? _random.Next() : 0; // An additional salt used during retries to avoid generating the same obfuscated 'base' string when extending the string length
        var seed = plaintext.GetHashCode() ^ _instanceSalt ^ salt;
        var random = new Random(seed);
        var retryLenght = Math.Max(0, retryCount - RetryLimitBeforeExtension);
        var obfuscated = new char[plaintext.Length + retryLenght];

        for (var i = 0; i < obfuscated.Length; i++)
        {
            if (i < plaintext.Length && IsReservedChar(plaintext[i]))
            {
                obfuscated[i] = plaintext[i]; // Do not obfuscate reserved characters
                continue;
            }
            
            if (i == 0) // Always use an alphabetic char for the first char to avoid generating invalid DAX identifiers
            {
                obfuscated[i] = AlphaCharSet[random.Next(AlphaCharSet.Length)];
                continue;
            }

            obfuscated[i] = CharSet[random.Next(CharSet.Length)];
        }

        var obfuscatedValue = new string(obfuscated);
        Debug.WriteLineIf(retryCount > 0, $"\t>> Retry {retryCount} for: {text.Value} | {text.ObfuscatedValue} > {obfuscatedValue}");

        text.ObfuscatedValue = obfuscatedValue;
        return text;
    }

    private static bool IsReservedChar(char @char)
    {
        // Reserved characters are preserved during obfuscation

        switch (@char)
        {
            case ReservedChar_Minus: // single-line comment char
            case '/': // multi-line comment char
            case '*': // multi-line comment char
            case '\n': // line feed char e.g. in multi-line comments
            case '\r': // carriage return char e.g. in multi-line comments
                return true;
        }

        return false;
    }

    private static readonly HashSet<string> ReservedTokens = new(StringComparer.OrdinalIgnoreCase)
    {
        ReservedToken_Date,
        ReservedToken_Value
    };

    internal const char ReservedChar_Minus = '-';
    /// <summary>
    /// CALENDAR() [Date] extension column.
    /// </summary>
    internal const string ReservedToken_Date = "Date";
    /// <summary>
    /// ''[Value] or table constructor { } extension columns
    /// </summary>
    internal const string ReservedToken_Value = "Value";
}
