﻿using System.Reflection;
using Dax.Metadata;
using Dax.Vpax.Obfuscator.Common;
using Dax.Vpax.Obfuscator.Extensions;

namespace Dax.Vpax.Obfuscator;

// TODO: (vNext) source generator
internal sealed partial class DaxModelObfuscator
{
    private readonly Model _model;
    private readonly DaxTextCollection _texts;
    private readonly DaxTextObfuscator _obfuscator;

    public DaxModelObfuscator(Model model, ObfuscationDictionary? dictionary = null)
    {
        if (model.IsObfuscated()) throw new InvalidOperationException("The model has already been obfuscated.");

        _model = model;
        _model.ObfuscatorDictionaryId = dictionary != null ? dictionary.Id : Guid.NewGuid().ToString("D");
        _model.ObfuscatorLib = "Dax.Vpax.Obfuscator"; // hard-coded
        _model.ObfuscatorLibVersion = GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? throw new InvalidOperationException("The assembly informational version is not available.");
        _obfuscator = new DaxTextObfuscator();
        _texts = new DaxTextCollection(dictionary);
    }

    public DaxTextCollection Texts => _texts;

    public void Obfuscate()
    {
        // Obfuscate and map identifiers first
        _model.Tables.ForEach(ObfuscateIdentifiers);

        Obfuscate(_model.ModelName);
        Obfuscate(_model.ServerName);
        _model.Tables.ForEach(Obfuscate);
        _model.Relationships.ForEach(Obfuscate);
        _model.Roles.ForEach(Obfuscate);
    }

    private void ObfuscateIdentifiers(Table table)
    {
        Obfuscate(table.TableName);
        table.Columns.ForEach(ObfuscateIdentifiers);
        table.Measures.ForEach(ObfuscateIdentifiers);
    }

    private void ObfuscateIdentifiers(Column column)
    {
        Obfuscate(column.ColumnName);
        Obfuscate(column.SourceColumn);
        Obfuscate(column.SortByColumnName);
        column.GroupByColumns.ForEach((n) => Obfuscate(n));
    }

    private void ObfuscateIdentifiers(Measure measure)
    {
        var measureText = Obfuscate(measure.MeasureName) ?? throw new InvalidOperationException($"The measure name is not valid [{measure.MeasureName}].");
        CreateKpiMeasure(measure.KpiTargetExpression, "Goal");
        CreateKpiMeasure(measure.KpiStatusExpression, "Status");
        CreateKpiMeasure(measure.KpiTrendExpression, "Trend");

        void CreateKpiMeasure(DaxExpression kpi, string type)
        {
            if (string.IsNullOrWhiteSpace(kpi?.Expression)) return;

            var text = new DaxText($"_{measureText.Value} {type}");
            text.ObfuscatedValue = $"_{measureText.ObfuscatedValue} {type}";

            // It may already exist in case of incremental obfuscation
            if (Texts.IsIncrementalObfuscation && Texts.Contains(text))
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

    private DaxText? Obfuscate(DaxName name)
    {
        if (string.IsNullOrWhiteSpace(name?.Name)) return null;

        var text = ObfuscateText(new DaxText(name!.Name));
        name.Name = text.ObfuscatedValue;
        return text;
    }

    private void Obfuscate(DaxNote note)
    {
        if (string.IsNullOrWhiteSpace(note?.Note)) return;

        var text = ObfuscateText(new DaxText(note!.Note));
        note.Note = text.ObfuscatedValue;
    }

    private void Obfuscate(DaxExpression expression)
    {
        if (string.IsNullOrWhiteSpace(expression?.Expression)) return;

        var value = ObfuscateExpression(expression!.Expression);
        expression.Expression = value;
    }
}
