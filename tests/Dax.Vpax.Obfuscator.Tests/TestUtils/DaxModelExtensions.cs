using Dax.Metadata;

namespace Dax.Vpax.Obfuscator.Tests.TestUtils;

internal static class DaxModelExtensions
{
    public static Function AddFunction(this Model model, string name, string expression)
    {
        var function = new Function()
        {
            FunctionName = new DaxName(name),
            FunctionExpression = new DaxExpression(expression)
        };
        model.Functions.Add(function);
        return function;
    }

    public static Table AddTable(this Model model, string name, string? description = null, string? expression = null)
    {
        var table = new Table(model)
        {
            TableName = new DaxName(name),
            Description = new DaxNote(description),
            TableExpression = new DaxExpression(expression)
        };
        model.Tables.Add(table);
        return table;
    }

    public static Column AddColumn(this Table table, string name, string? description = null, string? expression = null)
    {
        var column = new Column(table)
        {
            ColumnName = new DaxName(name),
            Description = new DaxNote(description),
            ColumnExpression = new DaxExpression(expression)
        };
        table.Columns.Add(column);
        return column;
    }

    public static Measure AddMeasure(this Table table, string name, string? description = null, string? expression = null)
    {
        var measure = new Measure()
        {
            Table = table,
            MeasureName = new DaxName(name),
            Description = new DaxNote(description),
            MeasureExpression = new DaxExpression(expression)
        };
        table.Measures.Add(measure);
        return measure;
    }

    public static Measure AddKpiStatus(this Measure measure, string expression)
    {
        measure.KpiStatusExpression = new DaxExpression(expression);
        return measure;
    }

    public static Measure AddKpiTarget(this Measure measure, string expression)
    {
        measure.KpiTargetExpression = new DaxExpression(expression);
        return measure;
    }

    public static Measure AddKpiTrend(this Measure measure, string expression)
    {
        measure.KpiTrendExpression = new DaxExpression(expression);
        return measure;
    }
}
