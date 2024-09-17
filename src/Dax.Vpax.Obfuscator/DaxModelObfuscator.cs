using Dax.Metadata;
using Dax.Vpax.Obfuscator.Common;
using Dax.Vpax.Obfuscator.Extensions;

namespace Dax.Vpax.Obfuscator;

// TODO: (vNext) source generator
internal sealed partial class DaxModelObfuscator
{
    private readonly DaxTextObfuscator _obfuscator = new();

    public DaxModelObfuscator(ObfuscationOptions options, Model model, ObfuscationDictionary dictionary)
        : this(options, model)
    {
        Model.ObfuscatorDictionaryId = dictionary.Id;
        Texts = new DaxTextCollection(dictionary);
        Mode = ObfuscationMode.Incremental;
    }

    public DaxModelObfuscator(ObfuscationOptions options, Model model)
    {
        if (model.IsObfuscated()) throw new InvalidOperationException("The model has already been obfuscated.");

        Options = options;
        Model = model;
        Model.ObfuscatorDictionaryId = Guid.NewGuid().ToString("D");
        Model.ObfuscatorLib = "Dax.Vpax.Obfuscator"; // hard-coded
        Model.ObfuscatorLibVersion = VpaxObfuscator.Version;
    }

    public Model Model { get; } // test only
    public ObfuscationMode Mode { get; }
    public ObfuscationOptions Options { get; }
    public DaxTextCollection Texts { get; } = new();
    public List<string> UnobfuscatedValues { get; } = new();

    public ObfuscationDictionary Obfuscate()
    {
        // Obfuscate and map identifiers first
        Model.Tables.ForEach(ObfuscateIdentifiers);

        Obfuscate(Model.ModelName);
        Obfuscate(Model.ServerName);
        Model.Tables.ForEach(Obfuscate);
        Model.Relationships.ForEach(Obfuscate);
        Model.Roles.ForEach(Obfuscate);

        var id = Model.ObfuscatorDictionaryId;
        var version = VpaxObfuscator.Version;
        var texts = Texts.Select((t) => t.ToObfuscationText());
        var unobfuscatedValues = Options.TrackUnobfuscated ? UnobfuscatedValues.Distinct(StringComparer.OrdinalIgnoreCase) : [];

        return new ObfuscationDictionary(id, version, texts, unobfuscatedValues);
    }

    private void ObfuscateIdentifiers(Table table)
    {
        ObfuscateTableName(table.TableName);
        table.Columns.ForEach(ObfuscateIdentifiers);
        table.Measures.ForEach(ObfuscateIdentifiers);
    }

    private void ObfuscateIdentifiers(Column column)
    {
        ObfuscateColumnName(column.ColumnName);
        ObfuscateColumnName(column.SourceColumn);
        ObfuscateColumnName(column.SortByColumnName);
        column.GroupByColumns.ForEach(ObfuscateColumnName);
    }

    private void ObfuscateIdentifiers(Measure measure)
    {
        var name = measure.MeasureName?.Name ?? throw new InvalidOperationException("The measure name is null.");
        var obfuscatedName = ObfuscateMeasureName(measure.MeasureName) ?? throw new InvalidOperationException($"The measure name is not valid [{name}].");

        CreateKpiMeasureText(measure.KpiTargetExpression, "Goal");
        CreateKpiMeasureText(measure.KpiStatusExpression, "Status");
        CreateKpiMeasureText(measure.KpiTrendExpression, "Trend");

        void CreateKpiMeasureText(DaxExpression kpi, string type)
        {
            if (string.IsNullOrWhiteSpace(kpi?.Expression)) return;

            var value = $"_{name} {type}";
            var obfuscatedValue = $"_{obfuscatedName} {type}";
            var text = new DaxText(value, obfuscatedValue);

            // Only add the KPI measure if it does not exist. Can happen in case of incremental obfuscation
            if (Mode == ObfuscationMode.Incremental && Texts.Contains(text))
                return;

            Texts.Add(text);
        }
    }

    private void Obfuscate(Table table)
    {
        Obfuscate(table.TableExpression);
        Obfuscate(table.CalculationGroup);
        Obfuscate(table.Description);
        table.Columns.ForEach(Obfuscate);
        table.Measures.ForEach(Obfuscate);
        table.UserHierarchies.ForEach(Obfuscate);
        table.Partitions.ForEach(Obfuscate);
    }

    private void Obfuscate(Column column)
    {
        Obfuscate(column.ColumnExpression);
        Obfuscate(column.DisplayFolder);
        Obfuscate(column.Description);
        column.ColumnHierarchies.ForEach(Obfuscate);
        column.ColumnSegments.ForEach(Obfuscate);
    }

    private void Obfuscate(ColumnHierarchy columnHierarchy)
    {
        Obfuscate(columnHierarchy.StructureName);
    }

    private void Obfuscate(ColumnSegment columnSegment) { }

    private void Obfuscate(Measure measure)
    {
        Obfuscate(measure.MeasureExpression);
        Obfuscate(measure.FormatStringExpression);
        Obfuscate(measure.DisplayFolder);
        Obfuscate(measure.Description);
        Obfuscate(measure.DetailRowsExpression);
        Obfuscate(measure.KpiTargetExpression);
        Obfuscate(measure.KpiStatusExpression);
        Obfuscate(measure.KpiTrendExpression);
    }

    private void Obfuscate(UserHierarchy userHierarchy)
    {
        Obfuscate(userHierarchy.HierarchyName);
    }

    private void Obfuscate(Partition partition)
    {
        Obfuscate(partition.PartitionName);
        Obfuscate(partition.Description);
    }

    private void Obfuscate(Relationship relationship) { }

    private void Obfuscate(CalculationGroup calculationGroup)
    {
        if (calculationGroup == null)
            return;

        foreach (var calculationItem in calculationGroup.CalculationItems)
        {
            Obfuscate(calculationItem.ItemName);
            Obfuscate(calculationItem.ItemExpression);
            Obfuscate(calculationItem.Description);
        }
    }

    private void Obfuscate(Role role)
    {
        Obfuscate(role.RoleName);
        role.TablePermissions.ForEach(Obfuscate);
    }

    private void Obfuscate(TablePermission tablePermission)
    {
        Obfuscate(tablePermission.FilterExpression);
    }

    private void ObfuscateTableName(DaxName name) => Obfuscate(name, ObfuscationRule.PreserveDaxKeywords);
    private void ObfuscateColumnName(DaxName name) => Obfuscate(name, ObfuscationRule.PreserveDaxReservedNames);
    private string? ObfuscateMeasureName(DaxName name) => Obfuscate(name, ObfuscationRule.PreserveDaxReservedNames);

    private string? Obfuscate(DaxName name, ObfuscationRule rule = ObfuscationRule.None)
    {
        if (string.IsNullOrWhiteSpace(name?.Name)) return null;

        var value = ObfuscateText(new DaxText(name!.Name), rule);
        return name.Name = value;
    }

    private void Obfuscate(DaxNote note)
    {
        if (string.IsNullOrWhiteSpace(note?.Note)) return;

        var value = ObfuscateText(new DaxText(note!.Note));
        note.Note = value;
    }

    private void Obfuscate(DaxExpression expression)
    {
        if (string.IsNullOrWhiteSpace(expression?.Expression)) return;

        var value = ObfuscateExpression(expression!.Expression);
        expression.Expression = value;
    }
}
