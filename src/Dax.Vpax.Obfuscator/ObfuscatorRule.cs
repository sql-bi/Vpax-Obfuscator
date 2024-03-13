using Dax.Vpax.Obfuscator.Extensions;

namespace Dax.Vpax.Obfuscator;

internal enum ObfuscatorRule
{
    None,
    PreserveDaxKeywords,
    PreserveDaxReservedNames
}

internal static class ObfuscationRuleExtensions
{
    public static bool ShouldPreserve(this ObfuscatorRule rule, string value)
    {
        switch (rule)
        {
            case ObfuscatorRule.None: return false;
            case ObfuscatorRule.PreserveDaxKeywords: return value.IsDaxKeyword();
            case ObfuscatorRule.PreserveDaxReservedNames: return value.IsDaxReservedName();
            default: throw new InvalidOperationException($"Invalid obfuscator rule. {rule}");
        }
    }
}
