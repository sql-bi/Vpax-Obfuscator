using Dax.Vpax.Obfuscator.Extensions;

namespace Dax.Vpax.Obfuscator;

internal enum ObfuscationRule
{
    None,
    PreserveDaxKeywords,
    PreserveDaxReservedNames
}

internal static class ObfuscationRuleExtensions
{
    public static bool ShouldPreserve(this ObfuscationRule rule, string value)
    {
        switch (rule)
        {
            case ObfuscationRule.None: return false;
            case ObfuscationRule.PreserveDaxKeywords: return value.IsDaxKeyword();
            case ObfuscationRule.PreserveDaxReservedNames: return value.IsDaxReservedName();
            default: throw new InvalidOperationException($"Invalid obfuscator rule. {rule}");
        }
    }
}
