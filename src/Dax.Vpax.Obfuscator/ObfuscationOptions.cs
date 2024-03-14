namespace Dax.Vpax.Obfuscator;

public sealed class ObfuscationOptions
{
    public static ObfuscationOptions Default { get; } = new();

    /// <summary>
    /// Specifies whether to include unobfuscated values in the generated dictionary.
    /// </summary>
    public bool TrackUnobfuscated { get; set; } = true;
}
