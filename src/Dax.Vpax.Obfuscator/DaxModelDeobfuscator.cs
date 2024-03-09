using Dax.Metadata;
using Dax.Tokenizer;
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
        Deobfuscate(table.TableName);
        Deobfuscate(table.TableExpression);
        Deobfuscate(table.CalculationGroup);
        Deobfuscate(table.Description);
        table.Columns.ForEach(Deobfuscate);
        table.Measures.ForEach(Deobfuscate);
        table.UserHierarchies.ForEach(Deobfuscate);
        table.Partitions.ForEach(Deobfuscate);
    }

    private void Deobfuscate(Column column)
    {
        Deobfuscate(column.ColumnName);
        Deobfuscate(column.SourceColumn);
        Deobfuscate(column.SortByColumnName);
        Deobfuscate(column.ColumnExpression);
        Deobfuscate(column.DisplayFolder);
        Deobfuscate(column.Description);
        column.ColumnHierarchies.ForEach(Deobfuscate);
        column.ColumnSegments.ForEach(Deobfuscate);
        column.GroupByColumns.ForEach(Deobfuscate);
    }

    private void Deobfuscate(ColumnHierarchy columnHierarchy)
    {
        Deobfuscate(columnHierarchy.StructureName);
    }

    private void Deobfuscate(ColumnSegment columnSegment) { }

    private void Deobfuscate(Measure measure)
    {
        Deobfuscate(measure.MeasureName);
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

    private void Deobfuscate(DaxName name)
    {
        if (string.IsNullOrWhiteSpace(name?.Name)) return;

        if (name!.Name.TryGetTableAndColumnNames(out var table, out var column))
        {
            var value = DeobfuscateTableAndColumnNames(table, column);
            name.Name = value;
        }
        else
        {
            var value = _dictionary.GetValue(name!.Name);
            name.Name = value;
        }
    }

    private void Deobfuscate(DaxNote note)
    {
        if (string.IsNullOrWhiteSpace(note?.Note)) return;

        var value = _dictionary.GetValue(note!.Note);
        note.Note = value;
    }

    private void Deobfuscate(DaxExpression expression)
    {
        if (string.IsNullOrWhiteSpace(expression?.Expression)) return;

        var value = DeobfuscateExpression(expression!.Expression);
        expression.Expression = value;
    }
}
