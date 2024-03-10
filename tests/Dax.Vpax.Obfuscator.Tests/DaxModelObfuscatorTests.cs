using System.Diagnostics;
using System.Globalization;
using Dax.Metadata;
using Dax.Tokenizer;
using Dax.Vpax.Obfuscator.Common;
using Dax.Vpax.Obfuscator.Tests.TestUtils;
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
    public void ObfuscateExpression_TableConstructorColumnNameSingle_IsNotObfuscated()
    {
        var expression = """ SELECTCOLUMNS({0}, "@c1", [Value]) """;
        var expected   = """ SELECTCOLUMNS({0}, "XXX", [Value]) """;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("@c1", "XXX"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_TableConstructorColumnNameMultiple_IsNotObfuscated()
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
    public void ObfuscateExpression_ExtensionColumnName_Test1()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "@c", 1), [@c]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "XX", 1), [XX]) """;
        var expected_d = expression;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("@c", "XX"));

        var actual_o = obfuscator.ObfuscateExpression(expression);
        Assert.Equal(expected_o, actual_o);

        var actual_d = GetDeobfuscator(obfuscator).DeobfuscateExpression(actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_Test2()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "t[c]", 1), t[c]) """;
        var expected   = """ SUMX(ADDCOLUMNS({}, "X[Y]", 1), X[Y]) """;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("t", "X"));
        obfuscator.Texts.Add(new DaxText("c", "Y"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_Test3()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "'t'[c]", 1), t[c]) """;
        var expected   = """ SUMX(ADDCOLUMNS({}, "'X'[Y]", 1), X[Y]) """;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("t", "X"));
        obfuscator.Texts.Add(new DaxText("c", "Y"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_Test4()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "t[c]", 1), 't'[c]) """;
        var expected = """ SUMX(ADDCOLUMNS({}, "X[Y]", 1), 'X'[Y]) """;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("t", "X"));
        obfuscator.Texts.Add(new DaxText("c", "Y"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_Test5()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "c[""]", 1), c["]) """;
        var expected   = """ SUMX(ADDCOLUMNS({}, "X[Y]", 1), X[Y]) """;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("c", "X"));
        obfuscator.Texts.Add(new DaxText("\"", "Y"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_Test6()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, " t [c]", 1), t[c]) """;
        var expected   = """ SUMX(ADDCOLUMNS({}, "X[Y]", 1), X[Y]) """;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("t", "X"));
        obfuscator.Texts.Add(new DaxText("c", "Y"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_Test7()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, " c ", 1), [ c ]) """;
        var expected   = """ SUMX(ADDCOLUMNS({}, "XXX", 1), [XXX]) """;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText(" c ", "XXX"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_Test8()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, " t [ c ]", 1), t[ c ]) """;
        var expected   = """ SUMX(ADDCOLUMNS({}, "X[YYY]", 1), X[YYY]) """;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("t", "X"));
        obfuscator.Texts.Add(new DaxText(" c ", "YYY"));
        var actual = obfuscator.ObfuscateExpression(expression);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_Test9()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, " t [ c@ ]", 1), t[ c@ ]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "X[YYYY]", 1), X[YYYY]) """;
        var expected_d = """ SUMX(ADDCOLUMNS({}, "t[ c@ ]", 1), t[ c@ ]) """;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("t", "X"));
        obfuscator.Texts.Add(new DaxText(" c@ ", "YYYY"));

        var actual_o = obfuscator.ObfuscateExpression(expression);
        Assert.Equal(expected_o, actual_o);

        var actual_d = GetDeobfuscator(obfuscator).DeobfuscateExpression(actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_Test10()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "c[ ] ]", 1), [c[ ]] ]]]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "XXXXXX", 1), [XXXXXX]) """;
        var expected_d = expression;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("c[ ] ]", "XXXXXX"));

        var actual_o = obfuscator.ObfuscateExpression(expression);
        Assert.Equal(expected_o, actual_o);

        var actual_d = GetDeobfuscator(obfuscator).DeobfuscateExpression(actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_Test11()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "c""1[a]", 1), [c"1[a]]]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "XXXXXX", 1), [XXXXXX]) """;
        var expected_d = expression;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("c\"1[a]", "XXXXXX"));

        var actual_o = obfuscator.ObfuscateExpression(expression);
        Assert.Equal(expected_o, actual_o);

        var actual_d = GetDeobfuscator(obfuscator).DeobfuscateExpression(actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_Test12()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "c'1[a]", 1), [c'1[a]]]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "XXXXXX", 1), [XXXXXX]) """;
        var expected_d = expression;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("c'1[a]", "XXXXXX"));

        var actual_o = obfuscator.ObfuscateExpression(expression);
        Assert.Equal(expected_o, actual_o);

        var actual_d = GetDeobfuscator(obfuscator).DeobfuscateExpression(actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_Test13()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "' t [tc]'[c]", 1), ' t [tc]'[c]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "'XXXXXXX'[Z]", 1), 'XXXXXXX'[Z]) """;
        var expected_d = expression;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText(" t [tc]", "XXXXXXX"));
        obfuscator.Texts.Add(new DaxText("c", "Z"));

        var actual_o = obfuscator.ObfuscateExpression(expression);
        Assert.Equal(expected_o, actual_o);

        var actual_d = GetDeobfuscator(obfuscator).DeobfuscateExpression(actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_Test14()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "' t [tc]'[ c ' ''"" [[ [ ]] ]", 1), ' t [tc]'[ c ' ''" [[ [ ]] ]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "'XXXXXXX'[ZZZZZZZZZZZZZZZZ]", 1), 'XXXXXXX'[ZZZZZZZZZZZZZZZZ]) """;
        var expected_d = expression;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText(" t [tc]", "XXXXXXX"));
        obfuscator.Texts.Add(new DaxText(" c ' ''\" [[ [ ] ", "ZZZZZZZZZZZZZZZZ"));

        var actual_o = obfuscator.ObfuscateExpression(expression);
        Assert.Equal(expected_o, actual_o);

        var actual_d = GetDeobfuscator(obfuscator).DeobfuscateExpression(actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_Test15()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "c''1[a]", 1), [c''1[a]]]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "XXXXXXX", 1), [XXXXXXX]) """;
        var expected_d = expression;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("c''1[a]", "XXXXXXX"));

        var actual_o = obfuscator.ObfuscateExpression(expression);
        Assert.Equal(expected_o, actual_o);

        var actual_d = GetDeobfuscator(obfuscator).DeobfuscateExpression(actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_Test16()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, " c ", 1), [ c ]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "XXX", 1), [XXX]) """;
        var expected_d = expression;

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText(" c ", "XXX"));

        var actual_o = obfuscator.ObfuscateExpression(expression);
        Assert.Equal(expected_o, actual_o);

        var actual_d = GetDeobfuscator(obfuscator).DeobfuscateExpression(actual_o);
        Assert.Equal(expected_d, actual_d);
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
        var expected   = "RELATED( XXXXX[YYYYYYY] )";

        var obfuscator = new DaxModelObfuscator(new Model());
        obfuscator.Texts.Add(new DaxText("Sales", "XXXXX"));
        obfuscator.Texts.Add(new DaxText("Rate[%]", "YYYYYYY"));
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

    [DebuggerStepThrough]
    private static DaxModelDeobfuscator GetDeobfuscator(DaxModelObfuscator obfuscator)
    {
        var texts = obfuscator.Texts.Select((t) => t.ToObfuscationText()).ToArray();
        var dictionary = new ObfuscationDictionary(id: Guid.NewGuid().ToString("D"), texts);
        obfuscator.Model.ObfuscatorDictionaryId = dictionary.Id;
        return new DaxModelDeobfuscator(obfuscator.Model, dictionary);
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
