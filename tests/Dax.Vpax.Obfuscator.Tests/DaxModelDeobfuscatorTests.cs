using Dax.Metadata;
using Dax.Vpax.Obfuscator.Common;
using Dax.Vpax.Obfuscator.Tests.TestUtils;
using Xunit;

namespace Dax.Vpax.Obfuscator.Tests;

public class DaxModelDeobfuscatorTests
{
    [Fact]
    public void Deobfuscate_KpiMeasureReference_DoesNotDeobfuscateKpiMeasureNamePrefixAndSuffixBecauseTheyAreNotObfuscated()
    {
        var expression = "[_XXXXXX Goal] + [_XXXXXX Trend] + [_XXXXXX Status]";
        var expected   = "[_Amount Goal] + [_Amount Trend] + [_Amount Status]";

        var (model, _, deobfuscator) = CreateTest(
        [
            new ObfuscationText("_", "Y"),
            new ObfuscationText("Amount", "XXXXXX"),
            new ObfuscationText("_Amount Goal", "_XXXXXX Goal"),
            new ObfuscationText("_Amount Trend", "_XXXXXX Trend"),
            new ObfuscationText("_Amount Status", "_XXXXXX Status"),
        ]);
        var table = model.AddTable("Y");
        _ = table.AddMeasure("XXXXXX", expression: "1").AddKpiTarget("1").AddKpiStatus("1").AddKpiTrend("1");
        var measure = table.AddMeasure("Y", expression: expression);

        deobfuscator.Deobfuscate();
        Assert.Equal(expected, measure.MeasureExpression.Expression);
    }

    [Fact]
    public void DeobfuscateExpression_CalendarDateColumn_IsNotDeobfuscatedBecauseItIsNotObfuscated()
    {
        var expression = """ SELECTCOLUMNS(CALENDAR(1, 100), "XXX", [Date]) """;
        var expected   = """ SELECTCOLUMNS(CALENDAR(1, 100), "@c1", [Date]) """;

        var (_, _, deobfuscator) = CreateTest(
        [
            new ObfuscationText("@c1", "XXX"),
        ]);
        var actual = deobfuscator.DeobfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DeobfuscateExpression_TableConstructorColumnName_IsNotDeobfuscatedBecauseItIsNotObfuscated_SingleColumnTest()
    {
        var expression = """ SELECTCOLUMNS({0}, "XXX", [Value]) """;
        var expected   = """ SELECTCOLUMNS({0}, "@c1", [Value]) """;

        var (_, _, deobfuscator) = CreateTest(
        [
            new ObfuscationText("@c1", "XXX"),
        ]);
        var actual = deobfuscator.DeobfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DeobfuscateExpression_TableConstructorColumnName_IsNotDeobfuscatedBecauseItIsNotObfuscated_MultipleColumnsTest()
    {
        var expression = """ SELECTCOLUMNS({(1,2,3)}, "XXX", [Value1], "YYY", [value2], "ZZZ", [VALUE3]) """;
        var expected   = """ SELECTCOLUMNS({(1,2,3)}, "@c1", [Value1], "@c2", [value2], "@c3", [VALUE3]) """;

        var (_, _, deobfuscator) = CreateTest(
        [
            new ObfuscationText("@c1", "XXX"),
            new ObfuscationText("@c2", "YYY"),
            new ObfuscationText("@c3", "ZZZ"),
        ]);
        var actual = deobfuscator.DeobfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DeobfuscateExpression_ExtensionColumnNameMultipleReferencesWithDifferentCasings_ReturnsSameDeobfuscatedValue()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "XXXXXXX", 1), [XXXXXXX]) """;
        var expected   = """ SUMX(ADDCOLUMNS({}, "@column", 1), [@COLUMN]) """;

        var (_, _, deobfuscator) = CreateTest(
        [
            new ObfuscationText("@column", "XXXXXXX"),
        ]);
        var actual = deobfuscator.DeobfuscateExpression(expression);

        Assert.Equal(expected, actual, ignoreCase: true);
    }

    [Fact]
    public void DeobfuscateExpression_ExtensionColumnNameFullyQualified_ReturnsDeobfuscatedColumnNameParts()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "XXXX[Y]", 1), XXXX[Y]) """;
        var expected   = """ SUMX(ADDCOLUMNS({}, "rate[%]", 1), rate[%]) """;

        var (_, _, deobfuscator) = CreateTest(
        [
            new ObfuscationText("rate", "XXXX"),
            new ObfuscationText("%", "Y"),
        ]);
        var actual = deobfuscator.DeobfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DeobfuscateExpression_ExtensionColumnNameFullyQualified_ReturnsDeobfuscatedColumnNamePartsWithoutPreservingQuotationMarkEscapeChar()
    {
        var expression = """ SELECTCOLUMNS(ADDCOLUMNS({}, "XXX[YYY]", 1), XXX[YYY]) """;
        var expected   = """ SELECTCOLUMNS(ADDCOLUMNS({}, "aaa[b""c]", 1), aaa[b"c]) """;

        var (_, _, deobfuscator) = CreateTest(
        [
            new ObfuscationText("aaa", "XXX"),
            new ObfuscationText("b\"c", "YYY"),
        ]);
        var actual = deobfuscator.DeobfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DeobfuscateExpression_TableNameMultipleReferencesWithDifferentCasings_ReturnsSameDeobfuscatedValue()
    {
        var expression = "COUNTROWS('XXXXX') + COUNTROWS(XXXXX) + COUNTROWS(XXXXX) + COUNTROWS(XXXXX)";
        var expected   = "COUNTROWS('Sales') + COUNTROWS(sales) + COUNTROWS(SALES) + COUNTROWS(SaLeS)";

        var (_, _, deobfuscator) = CreateTest(
        [
            new ObfuscationText("Sales", "XXXXX"),
        ]);
        var actual = deobfuscator.DeobfuscateExpression(expression);

        Assert.Equal(expected, actual, ignoreCase: true);
    }

    [Fact]
    public void DeobfuscateExpression_ColumnName_ReturnsDeobfuscatedValuePreservingSquareBracketEscapeChar()
    {
        var expression = "RELATED( XXXXX[YYYY[Z]]] )";
        var expected   = "RELATED( Sales[Rate[%]]] )";

        var (_, _, deobfuscator) = CreateTest(
        [
            new ObfuscationText("Sales", "XXXXX"),
            new ObfuscationText("Rate", "YYYY"),
            new ObfuscationText("%", "Z")
        ]);
        var actual = deobfuscator.DeobfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DeobfuscateExpression_VariableNameMultipleReferencesWithDifferentCasings_ReturnsSameDeobfuscatedValue()
    {
        var expression = "VAR XXXXXX = 1 RETURN XXXXXX + XXXXXX + XXXXXX";
        var expected   = "VAR Amount = 1 RETURN AMOUNT + AmOuNt + amount";

        var (_, _, deobfuscator) = CreateTest(
        [
            new ObfuscationText("Amount", "XXXXXX"),
        ]);
        var actual = deobfuscator.DeobfuscateExpression(expression);

        Assert.Equal(expected, actual, ignoreCase: true);
    }

    [Fact]
    public void DebfuscateExpression_ValueExtensionColumnName_IsNotDeobfuscatedBecauseItIsNotObfuscated()
    {
        var expression = """ SELECTCOLUMNS({0}, "XXXXXXXXXX", ''[Value]) """;
        var expected   = """ SELECTCOLUMNS({0}, "__Measures", ''[Value]) """;

        var (_, _, deobfuscator) = CreateTest(
        [
            new ObfuscationText("__Measures", "XXXXXXXXXX"),
        ]);
        var actual = deobfuscator.DeobfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DeobfuscateExpression_EmptyStringLiteral_IsNotDeobfuscatedBecauseItIsNotObfuscated()
    {
        var expected = """ IF("" = "", "", "") """;

        var (_, _, deobfuscator) = CreateTest([]);
        var actual = deobfuscator.DeobfuscateExpression(expected);

        Assert.Equal(expected, actual);
    }

    private (Model model, ObfuscationDictionary dictionary, DaxModelDeobfuscator deobfuscator) CreateTest(ObfuscationText[] texts)
    {
        var dictionary = new ObfuscationDictionary(id: Guid.NewGuid().ToString("D"), texts);
        var model = new Model
        {
            ObfuscatorDictionaryId = dictionary.Id,
            ObfuscatorLib = "TestLib",
            ObfuscatorLibVersion = "1.0.0",
        };
        var deobfuscator = new DaxModelDeobfuscator(model, dictionary);
        return (model, dictionary, deobfuscator);
    }
}
