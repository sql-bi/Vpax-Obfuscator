using System.Globalization;
using Dax.Metadata;
using Dax.Vpax.Obfuscator.Tests.TestUtils;
using Dax.Tokenizer;
using Xunit;

namespace Dax.Vpax.Obfuscator.Tests;

public class DaxModelObfuscatorTests
{
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

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("@c1", "XXX"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_TableConstructorColumnName_IsNotObfuscated_SingleColumnTest()
    {
        var expression = """ SELECTCOLUMNS({0}, "@c1", [Value]) """;
        var expected   = """ SELECTCOLUMNS({0}, "XXX", [Value]) """;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("@c1", "XXX"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_TableConstructorColumnName_IsNotObfuscated_MultipleColumnsTest()
    {
        var expression = """ SELECTCOLUMNS({(1,2,3)}, "@c1", [Value1], "@c2", [value2], "@c3", [VALUE3]) """;
        var expected =   """ SELECTCOLUMNS({(1,2,3)}, "XXX", [Value1], "YYY", [value2], "ZZZ", [VALUE3]) """;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("@c1", "XXX"));
        obfuscator.Texts.Add(new DaxText("@c2", "YYY"));
        obfuscator.Texts.Add(new DaxText("@c3", "ZZZ"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnNameMultipleReferencesWithDifferentCasings_ReturnsSameObfuscatedValue()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "@column", 1), [@COLUMN]) """;
        var expected   = """ SUMX(ADDCOLUMNS({}, "XXXXXXX", 1), [XXXXXXX]) """;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("@column", "XXXXXXX"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnNameFullyQualified_ReturnsObfuscatedColumnNameParts()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "rate[%]", 1), rate[%]) """;
        var expected   = """ SUMX(ADDCOLUMNS({}, "XXXX[Y]", 1), XXXX[Y]) """;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("rate", "XXXX"));
        obfuscator.Texts.Add(new DaxText("%", "Y"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_T1()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, " col5 [%]", 1), col5 [%]) """;
        var expected   = """ SUMX(ADDCOLUMNS({}, "XXXX[Y]", 1), XXXX[Y]) """;

        var obfuscator = new DaxModelObfuscator(new Model());
        //obfuscator.Texts.Add(new DaxText("rate", "XXXX"));
        //obfuscator.Texts.Add(new DaxText("%", "Y"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnNameFullyQualifiedWithSquareBracket_ReturnsObfuscatedColumnNameParts()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "@rate[%]", 1), [@rate[%]]]) """;
        var expected   = """ SUMX(ADDCOLUMNS({}, "XXXXX[Y]", 1), [XXXXX[Y]]]) """;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("@rate", "XXXXX"));
        obfuscator.Texts.Add(new DaxText("%", "Y"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnNameFullyQualified_ReturnsObfuscatedColumnNamePartsWithoutPreservingQuotationMarkEscapeChar()
    {
        var expression = """ SELECTCOLUMNS(ADDCOLUMNS({}, "aaa[b""c]", 1), aaa[b"c]) """;
        var expected   = """ SELECTCOLUMNS(ADDCOLUMNS({}, "XXX[YYY]", 1), XXX[YYY]) """;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("aaa", "XXX"));
        obfuscator.Texts.Add(new DaxText("b\"c", "YYY"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_TableNameWithDifferentCasings_ReturnsSameObfuscatedValue()
    {
        var expression = "COUNTROWS('Sales') + COUNTROWS(sales) + COUNTROWS(SALES) + COUNTROWS(SaLeS)";
        var expected   = "COUNTROWS('XXXXX') + COUNTROWS(XXXXX) + COUNTROWS(XXXXX) + COUNTROWS(XXXXX)";

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("Sales", "XXXXX"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ColumnName_ReturnsObfuscatedValuePreservingSquareBracketEscapeChar()
    {
        var expression = "RELATED( Sales[Rate[%]]] )";
        var expected   = "RELATED( XXXXX[YYYY[Z]]] )";

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("Sales", "XXXXX"));
        obfuscator.Texts.Add(new DaxText("Rate", "YYYY"));
        obfuscator.Texts.Add(new DaxText("%", "Z"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_VariableNameMultipleReferencesWithDifferentCasings_ReturnsSameObfuscatedValue()
    {
        var expression = "VAR Amount = 1 RETURN AMOUNT + AmOuNt + amount";
        var expected   = "VAR XXXXXX = 1 RETURN XXXXXX + XXXXXX + XXXXXX";

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("Amount", "XXXXXX"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ValueExtensionColumnName_IsNotObfuscated()
    {
        var expression = """ SELECTCOLUMNS({0}, "__Measures", ''[Value]) """;
        var expected =   """ SELECTCOLUMNS({0}, "XXXXXXXXXX", ''[Value]) """;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("__Measures", "XXXXXXXXXX"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_StringLiteralEmpty_IsNotObfuscated()
    {
        var expected = """ IF("" = "", "", "") """;

        var obfuscator = new DaxModelObfuscator(new Model());
        var actual = obfuscator.ObfuscateExpression(expected);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_StringLiteralWithEscapedQuotationMark_IsObfuscated()
    {
        var expression = """"" "A" & """" & "B" """"";
        var expected   = """"" "X" & "Y" & "Z" """"";

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("A", "X"));
        obfuscator.Texts.Add(new DaxText("\"", "Y"));
        obfuscator.Texts.Add(new DaxText("B", "Z"));
        var actual = obfuscator.ObfuscateExpression(expression);

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

        var obfuscator = new DaxModelObfuscator(new Model());
        var actual = obfuscator.ObfuscateExpression(expected);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Amount")]
    [InlineData("Sales Amount")]
    [InlineData("__myvar")]
    public void ObfuscateText_ReobfuscatingObfuscatedText_DoesNotChangeObfuscatedValue(string value)
    {
        var obfuscator = new DaxModelObfuscator(new Model());
        var text = obfuscator.ObfuscateText(new DaxText(value));
        var expected = text.ObfuscatedValue;

        // re-obfuscating the same text should not change the obfuscated value
        _ = obfuscator.ObfuscateText(text);
        _ = obfuscator.ObfuscateText(text);
        _ = obfuscator.ObfuscateText(text); 

        Assert.Single(obfuscator.Texts);
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
        var obfuscator = new DaxModelObfuscator(new Model());

        foreach (var value in values) {
            var text = new DaxText(value);
            _ = obfuscator.ObfuscateText(text); // << this throws in case of unresolved conflict
        }

        Assert.Equal(values.Count, obfuscator.Texts.Count);
    }

    [Fact]
    public void ObfuscateText_SingleChar_IsExtended()
    {
        var obfuscator = new DaxModelObfuscator(new Model());

        // Seed the dictionary
        foreach (var @char in "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
            obfuscator.Texts.Add(new DaxText(@char.ToString(), @char.ToString()));

        // Here the obfuscator starts extending the obfuscated value length to resolve conflicts
        foreach (var @char in "0123456789=1£$%&(){+")
        {
            var text = obfuscator.ObfuscateText(new DaxText(@char.ToString()));
            Assert.True(text.ObfuscatedValue.Length > 1);
        }
    }

    private static IEnumerable<char> GetUnicodeChars(params UnicodeCategory[] categories)
    {
        foreach (var category in categories)
        {
            for (var codepoint = 0; codepoint < ushort.MaxValue; codepoint++)
            {
                var @char = (char)codepoint;
                if (CharUnicodeInfo.GetUnicodeCategory(@char) == category)
                    yield return @char;
            }
        }
    }
}
