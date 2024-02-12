using System.Globalization;
using Dax.Metadata;
using Dax.Vpax.Obfuscator.Tests.TestUtils;
using TabularEditor.Dax.Tokenizer;
using Xunit;

namespace Dax.Vpax.Obfuscator.Tests;

public class DaxModelObfuscatorTests
{
    private readonly DaxModelObfuscator _obfuscator = new(new Model());

    [Fact]
    public void Obfuscate_KpiMeasureReference_DoesNotObfuscateKpiMeasureNamePrefixAndSuffix()
    {
        var expression = "[_Amount Goal] + [_Amount Trend] + [_Amount Status]";
        var expected   = "[_XXXXXX Goal] + [_XXXXXX Trend] + [_XXXXXX Status]";

        var model = new Model();
        var table = model.AddTable("_");
        _ = table.AddMeasure("Amount", expression: "1").AddKpiTarget("1").AddKpiStatus("1").AddKpiTrend("1");
        var measure = table.AddMeasure("_", expression: expression);

        var obfuscator = new DaxModelObfuscator(model);
        obfuscator.Texts.Add(new DaxText("Amount", "XXXXXX"));
        obfuscator.Obfuscate();

        Assert.Equal(expected, measure.MeasureExpression.Expression);
    }

    [Fact]
    public void ObfuscateExpression_CalendarDateColumn_IsNotObfuscated()
    {
        var expression = """ SELECTCOLUMNS(CALENDAR(1, 100), "@c1", [Date]) """;
        var expected   = """ SELECTCOLUMNS(CALENDAR(1, 100), "XXX", [Date]) """;

        _obfuscator.Texts.Add(new DaxText("@c1", "XXX"));
        var actual = _obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_TableConstructorColumnName_IsNotObfuscated_SingleColumnTest()
    {
        var expression = """ SELECTCOLUMNS({ (0) }, "@c1", [Value]) """;
        var expected   = """ SELECTCOLUMNS({ (0) }, "XXX", [Value]) """;

        _obfuscator.Texts.Add(new DaxText("@c1", "XXX"));
        var actual = _obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_TableConstructorColumnName_IsNotObfuscated_MultipleColumnsTest()
    {
        var expression = """ SELECTCOLUMNS({ (1,2,3) }, "@c1", [Value1], "@c2", [value2], "@c3", [VALUE3]) """;
        var expected =   """ SELECTCOLUMNS({ (1,2,3) }, "XXX", [Value1], "YYY", [value2], "ZZZ", [VALUE3]) """;

        _obfuscator.Texts.Add(new DaxText("@c1", "XXX"));
        _obfuscator.Texts.Add(new DaxText("@c2", "YYY"));
        _obfuscator.Texts.Add(new DaxText("@c3", "ZZZ"));
        var actual = _obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnNameWithDifferentCasings_ReturnsSameObfuscatedValue()
    {
        var expression = """ SUMX(ADDCOLUMNS(Date, "@column", 1), [@COLUMN]) """;
        var expected   = """ SUMX(ADDCOLUMNS(Date, "XXXXXXX", 1), [XXXXXXX]) """;

        _obfuscator.Texts.Add(new DaxText("@column", "XXXXXXX"));
        var actual = _obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_ReturnsObfuscatedTableNameAndColumnNameParts()
    {
        var expression = """ SUMX(ADDCOLUMNS(Date, "__rate[%]", 1), __rate[%]) """;
        var expected   = """ SUMX(ADDCOLUMNS(Date, "XXXXXX[Y]", 1), XXXXXX[Y]) """;

        _obfuscator.Texts.Add(new DaxText("__rate", "XXXXXX"));
        _obfuscator.Texts.Add(new DaxText("%", "Y"));
        var actual = _obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_ReturnsObfuscatedValuePreservingEscapeChar()
    {
        var expression = """ SELECTCOLUMNS(ADDCOLUMNS(Date, "aaa[b""c]", 1), aaa[b"c]) """;
        var expected   = """ SELECTCOLUMNS(ADDCOLUMNS(Date, "XXX[Y""Y]", 1), XXX[Y"Y]) """;

        _obfuscator.Texts.Add(new DaxText("aaa", "XXX"));
        _obfuscator.Texts.Add(new DaxText("b\"c", "Y\"Y"));
        var actual = _obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_TableNameWithDifferentCasings_ReturnsSameObfuscatedValue()
    {
        var expression = "COUNTROWS('Sales') + COUNTROWS(sales) + COUNTROWS(SALES) + COUNTROWS(SaLeS)";
        var expected   = "COUNTROWS('XXXXX') + COUNTROWS(XXXXX) + COUNTROWS(XXXXX) + COUNTROWS(XXXXX)";

        _obfuscator.Texts.Add(new DaxText("Sales", "XXXXX"));
        var actual = _obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ColumnNameWithEscapeSquareBracket_ReturnsObfuscatedValuePreservingEscapeChar()
    {
        var expression = "RELATED( Sales[Rate[%]]] )";
        var expected   = "RELATED( XXXXX[YYYYYY]]] )";

        _obfuscator.Texts.Add(new DaxText("Sales", "XXXXX"));
        _obfuscator.Texts.Add(new DaxText("Rate[%]", "YYYYYY]"));
        var actual = _obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_VariableNameWithDifferentCasings_ReturnsSameObfuscatedValue()
    {
        var expression = "VAR Amount = 1 RETURN AMOUNT + AmOuNt + amount";
        var expected   = "VAR XXXXXX = 1 RETURN XXXXXX + XXXXXX + XXXXXX";

        _obfuscator.Texts.Add(new DaxText("Amount", "XXXXXX"));
        var actual = _obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ValueExtensionColumnName_IsNotObfuscated()
    {
        var expression = """ SELECTCOLUMNS ( { 1 }, "__Measures", ''[Value]) """;
        var expected =   """ SELECTCOLUMNS ( { 1 }, "XXXXXXXXXX", ''[Value]) """;

        _obfuscator.Texts.Add(new DaxText("__Measures", "XXXXXXXXXX"));
        var actual = _obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_EmptyStringLiteral_IsNotObfuscated()
    {
        var expected = """ IF("" = "", "", "") """;
        var actual = _obfuscator.ObfuscateExpression(expected);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(nameof(DaxToken.DISPLAYFOLDER))]
    [InlineData(nameof(DaxToken.FORMATSTRING))]
    [InlineData(nameof(DaxToken.DESCRIPTION))] // <- not a valid variable name but it's fine for the test
    [InlineData(nameof(DaxToken.VISIBLE))] // <----- not a valid variable name but it's fine for the test
    [InlineData(nameof(DaxToken.DATATYPE))]
    public void ObfuscateExpression_ReservedDaxToken_IsNotObfuscated(string name)
    {
        var expected = $""" VAR {name} = 0 RETURN {name} """;
        var actual = _obfuscator.ObfuscateExpression(expected);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Amount")]
    [InlineData("Sales Amount")]
    [InlineData("__myvar")]
    public void ObfuscateText_ReobfuscatingObfuscatedText_DoesNotChangeObfuscatedValue(string value)
    {
        var text = _obfuscator.ObfuscateText(new DaxText(value));
        var expected = text.ObfuscatedValue;

        // re-obfuscating the same text should not change the obfuscated value
        _ = _obfuscator.ObfuscateText(text);
        _ = _obfuscator.ObfuscateText(text);
        _ = _obfuscator.ObfuscateText(text); 

        Assert.Single(_obfuscator.Texts);
        Assert.Equal(expected, text.ObfuscatedValue);
    }

    [Fact]
    public void ObfuscateText_SingleUnicodeChar_DoesNotConflict()
    {
        var categories = new[]
        {
            UnicodeCategory.UppercaseLetter,
            //UnicodeCategory.LowercaseLetter, // Excute lowercase since we use case-insensitive comparison
        };
        var values = GetUnicodeChars(categories).Select(char.ToString).ToList();

        foreach (var value in values) {
            var text = new DaxText(value);
            _ = _obfuscator.ObfuscateText(text); // << this will throw in case of unresolved conflict
        }

        Assert.Equal(values.Count, _obfuscator.Texts.Count);
    }

    [Fact]
    public void ObfuscateText_SingleChar_IsExtended()
    {
        // Seed the dictionary
        foreach (var @char in "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
            _obfuscator.Texts.Add(new DaxText(@char.ToString(), @char.ToString()));

        // Here the obfuscator should start extending the obfuscated value length to resolve conflicts
        foreach (var @char in "0123456789=1£$%&(){+") {
            var text = _obfuscator.ObfuscateText(new DaxText(@char.ToString()));
            Assert.True(text.ObfuscatedValue.Length > 1);
        }
    }

    private static IEnumerable<char> GetUnicodeChars(params UnicodeCategory[] categories)
    {
        foreach (var category in categories) {
            for (var codepoint = 0; codepoint < ushort.MaxValue; codepoint++) {
                var @char = (char)codepoint;
                if (CharUnicodeInfo.GetUnicodeCategory(@char) == category)
                    yield return @char;
            }
        }
    }
}
