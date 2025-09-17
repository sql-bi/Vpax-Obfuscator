namespace Dax.Vpax.Obfuscator;

/// <summary>
/// This enum mirrors Microsoft.AnalysisServices.Tabular.ObjectType.ObjectType
/// but only includes a subset of values.
/// </summary>
internal enum NamedObjectType
{
    Null = 0,

    /// <summary>
    /// Microsoft.AnalysisServices.Tabular.ObjectType.Function
    /// </summary>
    Function = 63,
}
