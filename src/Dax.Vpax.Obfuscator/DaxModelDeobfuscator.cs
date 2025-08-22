using Dax.Metadata;
using Dax.Vpax.Obfuscator.Common;
using Dax.Vpax.Obfuscator.Extensions;

namespace Dax.Vpax.Obfuscator;

internal partial class DaxModelDeobfuscator
{
    private readonly Model _model;
    private readonly ObfuscationDictionary _dictionary;

    public DaxModelDeobfuscator(Model model, ObfuscationDictionary dictionary)
    {
        if (!model.IsObfuscated()) throw new InvalidOperationException("The model has not been obfuscated.");
        if (!model.IsObfuscatedWithDictionaryId(dictionary.Id)) throw new InvalidOperationException("The model has not been obfuscated with the specified dictionary.");

        _model = model;
        _dictionary = dictionary;
    }

    public void Deobfuscate()
    {
        Deobfuscate(_model.ModelName);
        Deobfuscate(_model.ServerName);
        _model.Tables.ForEach(Deobfuscate);
        _model.Relationships.ForEach(Deobfuscate);
        _model.Roles.ForEach(Deobfuscate);
    }

    private void Deobfuscate(Table table)
    {
        DeobfuscateTableName(table.TableName);
        Deobfuscate(table.TableExpression);
        Deobfuscate(table.DefaultDetailRowsExpression);
        Deobfuscate(table.CalculationGroup);
        Deobfuscate(table.Description);
        table.Columns.ForEach(Deobfuscate);
        table.Measures.ForEach(Deobfuscate);
        table.UserHierarchies.ForEach(Deobfuscate);
        table.Partitions.ForEach(Deobfuscate);
    }

    private void Deobfuscate(Column column)
    {
        DeobfuscateColumnName(column.ColumnName);
        DeobfuscateColumnName(column.SourceColumn);
        DeobfuscateColumnName(column.SortByColumnName);
        Deobfuscate(column.ColumnExpression);
        Deobfuscate(column.DisplayFolder);
        Deobfuscate(column.Description);
        column.ColumnHierarchies.ForEach(Deobfuscate);
        column.ColumnSegments.ForEach(Deobfuscate);
        column.GroupByColumns.ForEach(DeobfuscateColumnName);
    }

    private void Deobfuscate(ColumnHierarchy columnHierarchy)
    {
        Deobfuscate(columnHierarchy.StructureName);
    }

    private void Deobfuscate(ColumnSegment columnSegment) { }

    private void Deobfuscate(Measure measure)
    {
        DeobfuscateMeasureName(measure.MeasureName);
        Deobfuscate(measure.MeasureExpression);
        Deobfuscate(measure.FormatStringExpression);
        Deobfuscate(measure.DisplayFolder);
        Deobfuscate(measure.Description);
        Deobfuscate(measure.DetailRowsExpression);
        Deobfuscate(measure.KpiTargetExpression);
        Deobfuscate(measure.KpiStatusExpression);
        Deobfuscate(measure.KpiTrendExpression);
    }

    private void Deobfuscate(UserHierarchy userHierarchy)
    {
        Deobfuscate(userHierarchy.HierarchyName);
    }

    private void Deobfuscate(Partition partition)
    {
        Deobfuscate(partition.PartitionName);
        Deobfuscate(partition.Description);
    }

    private void Deobfuscate(Relationship relationship) { }

    private void Deobfuscate(CalculationGroup calculationGroup)
    {
        if (calculationGroup == null)
            return;

        Deobfuscate(calculationGroup.Description);
        Deobfuscate(calculationGroup.MultipleOrEmptySelectionExpression);
        Deobfuscate(calculationGroup.MultipleOrEmptySelectionExpressionDescription);
        Deobfuscate(calculationGroup.MultipleOrEmptySelectionFormatStringExpression);
        Deobfuscate(calculationGroup.NoSelectionExpression);
        Deobfuscate(calculationGroup.NoSelectionExpressionDescription);
        Deobfuscate(calculationGroup.NoSelectionFormatStringExpression);

        foreach (var calculationItem in calculationGroup.CalculationItems)
        {
            Deobfuscate(calculationItem.ItemName);
            Deobfuscate(calculationItem.ItemExpression);
            Deobfuscate(calculationItem.Description);
        }
    }

    private void Deobfuscate(Role role)
    {
        Deobfuscate(role.RoleName);
        role.TablePermissions.ForEach(Deobfuscate);
    }

    private void Deobfuscate(TablePermission tablePermission)
    {
        Deobfuscate(tablePermission.FilterExpression);
    }

    private void DeobfuscateTableName(DaxName name) => Deobfuscate(name, ObfuscationRule.PreserveDaxKeywords);
    private void DeobfuscateColumnName(DaxName name) => Deobfuscate(name, ObfuscationRule.PreserveDaxReservedNames);
    private void DeobfuscateMeasureName(DaxName name) => Deobfuscate(name, ObfuscationRule.PreserveDaxReservedNames);

    private void Deobfuscate(DaxName name, ObfuscationRule rule = ObfuscationRule.None)
    {
        if (string.IsNullOrWhiteSpace(name?.Name)) return;

        var value = DeobfuscateText(name!.Name, rule);
        name.Name = value;
    }

    private void Deobfuscate(DaxNote note)
    {
        if (string.IsNullOrWhiteSpace(note?.Note)) return;

        var value = DeobfuscateText(note!.Note);
        note.Note = value;
    }

    private void Deobfuscate(DaxExpression expression)
    {
        if (string.IsNullOrWhiteSpace(expression?.Expression)) return;

        var value = DeobfuscateExpression(expression!.Expression);
        expression.Expression = value;
    }
}
