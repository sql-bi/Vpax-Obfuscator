using System.Diagnostics;
using System.Globalization;
using Dax.Metadata;
using Dax.Tokenizer;
using Dax.Vpax.Obfuscator.Common;
using Dax.Vpax.Obfuscator.Comparers;
using Dax.Vpax.Obfuscator.Tests.TestUtils;
using Xunit;

namespace Dax.Vpax.Obfuscator.Tests;

public class DaxModelObfuscatorTests
{
    [Fact]
    public void Obfuscate_KpiMeasureReference_IsObfuscatedPreservingNamePrefixAndSuffix()
    {
        var expression = "[_Amount Goal] + [_Amount Trend] + [_Amount Status]";
        var expected_o = "[_XXXXXX Goal] + [_XXXXXX Trend] + [_XXXXXX Status]";
        var expected_d = expression;

        var model = new Model();
        var table = model.AddTable("T");
        var measure = table.AddMeasure("M", expression: expression);
        _ = table.AddMeasure("Amount", expression: "1").AddKpiTarget("1").AddKpiStatus("1").AddKpiTrend("1");

        DaxText[] texts =
        [
            new ("Amount", "XXXXXX"),
            new ("T", "Y"),
            new ("M", "Z")
        ];
        var obfuscator = CreateObfuscator(texts, model);
        var dictionary = obfuscator.Obfuscate();

        Assert.Equal(expected_o, measure.MeasureExpression.Expression);
        Assert.Contains(new DaxText("_Amount Goal", "_XXXXXX Goal"), obfuscator.Texts, DaxTextEqualityComparer.Instance);
        Assert.Contains(new DaxText("_Amount Trend", "_XXXXXX Trend"), obfuscator.Texts, DaxTextEqualityComparer.Instance);
        Assert.Contains(new DaxText("_Amount Status", "_XXXXXX Status"), obfuscator.Texts, DaxTextEqualityComparer.Instance);

        CreateDeobfuscator(dictionary, model).Deobfuscate();
        Assert.Equal(expected_d, measure.MeasureExpression.Expression);
    }

    [Theory]
    [MemberData(nameof(GetDaxKeywordData))]
    public void Obfuscate_TableNameMatchingDaxKeywordName_IsNotObufuscated(string keyword)
    {
        var model = new Model();
        var table = model.AddTable(keyword);

        var obfuscator = CreateObfuscator([], model);
        var dictionary = obfuscator.Obfuscate();

        Assert.DoesNotContain(new DaxText(keyword), obfuscator.Texts, DaxTextValueEqualityComparer.Instance);
        Assert.Equal(keyword, table.TableName.Name);

        CreateDeobfuscator(dictionary, model).Deobfuscate();
        Assert.Equal(keyword, table.TableName.Name);
    }

    [Theory]
    [MemberData(nameof(GetDaxReservedNameData))]
    public void Obfuscate_ColumnAndMeasureNameMatchingReservedName_IsNotObfuscated(string name)
    {
        var model = new Model();
        var table = model.AddTable("_");
        var column = table.AddColumn(name);
        var measure = table.AddMeasure(name, "1");

        var obfuscator = CreateObfuscator([], model);
        var dictionary = obfuscator.Obfuscate();

        Assert.DoesNotContain(new DaxText(name), obfuscator.Texts, DaxTextValueEqualityComparer.Instance);
        Assert.Equal(name, column.ColumnName.Name);
        Assert.Equal(name, measure.MeasureName.Name);

        CreateDeobfuscator(dictionary, model).Deobfuscate();
        Assert.Equal(name, column.ColumnName.Name);
        Assert.Equal(name, measure.MeasureName.Name);
    }

    [Theory]
    [MemberData(nameof(GetDaxKeywordData))]
    public void ObfuscateExpression_VariableAndTableReferenceNameMatchingDaxKeyword_IsNotObfuscated(string keyword)
    {
        // The generated expression may contain syntax errors, but it is fine for testing
        var expression = $" VAR {keyword} = 1 RETURN {keyword} + COUNTROWS('{keyword}') + COUNTROWS({keyword}) ";

        var obfuscator = CreateObfuscator();
        var actual_o = obfuscator.ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(obfuscator.Texts).DeobfuscateExpression(actual_o);

        Assert.Empty(obfuscator.Texts);
        Assert.Equal(expression, actual_o);
        Assert.Equal(expression, actual_d);
    }

    [Theory]
    [MemberData(nameof(GetDaxReservedNameData))]
    public void ObfuscateExpression_ColumnAndMeasureReferenceNameMatchingReservedName_IsNotObfuscated(string name)
    {
        var expression = $" SUM([{name}]) + CALCULATE([{name}]) + SUMX(GENERATESERIES(1, 10), ''[Value])";

        var obfuscator = CreateObfuscator();
        var actual_o = obfuscator.ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(obfuscator.Texts).DeobfuscateExpression(actual_o);

        Assert.Empty(obfuscator.Texts);
        Assert.Equal(expression, actual_o);
        Assert.Equal(expression, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ColumnReference_IsObfuscatedEscapingSquareBracketsInColumnName()
    {
        var expression = "RELATED( Sales[Rate[%]]] )";
        var expected_o = "RELATED( XXXXX[YYYYYYY] )";
        var expected_d = expression;

        DaxText[] texts =
        [
            new("Sales", "XXXXX"),
            new("Rate[%]", "YYYYYYY")
        ];
        var actual_o = CreateObfuscator(texts).ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(texts).DeobfuscateExpression(actual_o);

        Assert.Equal(expected_o, actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_IsObfuscated()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "c", 1), [c]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "X", 1), [X]) """;
        var expected_d = expression;

        DaxText[] texts = [new("c", "X")];
        var actual_o = CreateObfuscator(texts).ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(texts).DeobfuscateExpression(actual_o);

        Assert.Equal(expected_o, actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_IsObfuscatedPreservingWhitespaces()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, " c ", 1), [ c ]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "XXX", 1), [XXX]) """;
        var expected_d = expression;

        DaxText[] texts = [new(" c ", "XXX")];
        var actual_o = CreateObfuscator(texts).ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(texts).DeobfuscateExpression(actual_o);

        Assert.Equal(expected_o, actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_IsObfuscatedEscapingDoubleQuotes()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "col""umn", 1), [col"umn]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "XXXXXXX", 1), [XXXXXXX]) """;
        var expected_d = expression;

        DaxText[] texts = [new("col\"umn", "XXXXXXX")];
        var actual_o = CreateObfuscator(texts).ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(texts).DeobfuscateExpression(actual_o);

        Assert.Equal(expected_o, actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_IsObfuscatedWithoutEscapingSingleQuotes()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "col'umn", 1), [col'umn]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "XXXXXXX", 1), [XXXXXXX]) """;
        var expected_d = expression;

        DaxText[] texts = [new("col'umn", "XXXXXXX")];
        var actual_o = CreateObfuscator(texts).ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(texts).DeobfuscateExpression(actual_o);

        Assert.Equal(expected_o, actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnName_IsObfuscatedEscapingSquareBracketsInColumnReference()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "col]umn", 1), [col]]umn]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "XXXXXXX", 1), [XXXXXXX]) """;
        var expected_d = expression;

        DaxText[] texts = [new("col]umn", "XXXXXXX")];
        var actual_o = CreateObfuscator(texts).ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(texts).DeobfuscateExpression(actual_o);

        Assert.Equal(expected_o, actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnNameWithTableColumnNames_IsObfuscated()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "t[c]", 1), t[c]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "X[Y]", 1), X[Y]) """;
        var expected_d = expression;

        DaxText[] texts =
        [
            new("t", "X"),
            new("c", "Y"),
        ];
        var actual_o = CreateObfuscator(texts).ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(texts).DeobfuscateExpression(actual_o);

        Assert.Equal(expected_o, actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnNameWithTableColumnNames_IsObfuscatedPreservingTableQuotesInExtensionColumnName()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "'t'[c]", 1), t[c]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "'X'[Y]", 1), X[Y]) """;
        var expected_d = expression;

        DaxText[] texts =
        [
            new("t", "X"),
            new("c", "Y"),
        ];
        var actual_o = CreateObfuscator(texts).ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(texts).DeobfuscateExpression(actual_o);

        Assert.Equal(expected_o, actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnNameWithTableColumnNames_IsObfuscatedPreservingTableQuotesInColumnReference()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "t[c]", 1), 't'[c]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "X[Y]", 1), 'X'[Y]) """;
        var expected_d = expression;

        DaxText[] texts =
        [
           new("t", "X"),
           new("c", "Y"),
        ];
        var actual_o = CreateObfuscator(texts).ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(texts).DeobfuscateExpression(actual_o);

        Assert.Equal(expected_o, actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnNameWithTableColumnNames_IsObfuscatedUnescapingColumnQuotesInExtensionColumnName()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "t[""]", 1), t["]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "X[Y]", 1), X[Y]) """;
        var expected_d = expression;

        DaxText[] texts =
        [
           new("t", "X"),
           new("\"", "Y"),
        ];
        var actual_o = CreateObfuscator(texts).ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(texts).DeobfuscateExpression(actual_o);

        Assert.Equal(expected_o, actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnNameWithTableColumnNames_IsObfuscatedTrimmingUnquotedTableInExtensionColumnName()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, " t [c]", 1), t[c]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "X[Y]", 1), X[Y]) """;
        var expected_d = """ SUMX(ADDCOLUMNS({}, "t[c]", 1), t[c]) """;

        DaxText[] texts =
        [
           new("t", "X"),
           new("c", "Y"),
        ];
        var actual_o = CreateObfuscator(texts).ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(texts).DeobfuscateExpression(actual_o);

        Assert.Equal(expected_o, actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnNameWithTableColumnNames_IsObfuscatedPreservingColumnWhitespacesOnly()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, " t [ c ]", 1), t[ c ]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "X[YYY]", 1), X[YYY]) """;
        var expected_d = """ SUMX(ADDCOLUMNS({}, "t[ c ]", 1), t[ c ]) """;

        DaxText[] texts =
        [
           new("t", "X"),
           new(" c ", "YYY"),
        ];
        var actual_o = CreateObfuscator(texts).ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(texts).DeobfuscateExpression(actual_o);

        Assert.Equal(expected_o, actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnNameWithTableColumnNames_IsObfuscatedIgnoringSquareBracketsInTableName()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "'t[tc]'[c]", 1), 't[tc]'[c]) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "'XXXXX'[Y]", 1), 'XXXXX'[Y]) """;
        var expected_d = expression;

        DaxText[] texts =
        [
           new("t[tc]", "XXXXX"),
           new("c", "Y"),
        ];
        var actual_o = CreateObfuscator(texts).ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(texts).DeobfuscateExpression(actual_o);

        Assert.Equal(expected_o, actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_ExtensionColumnNameWithTableColumnNames_IsObfuscatedIgnoringSingleQuotesInColumnReference()
    {
        var expression = """ SUMX(ADDCOLUMNS({}, "'t'['c'']", 1), 't'['c'']) """;
        var expected_o = """ SUMX(ADDCOLUMNS({}, "'X'[YYYY]", 1), 'X'[YYYY]) """;
        var expected_d = expression;

        DaxText[] texts =
        [
           new("t", "X"),
           new("'c''", "YYYY"),
        ];
        var actual_o = CreateObfuscator(texts).ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(texts).DeobfuscateExpression(actual_o);

        Assert.Equal(expected_o, actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_DaxToken_IsObfuscatedCaseInsensitive()
    {
        var expression = "VAR AMOUNT = 1 RETURN AmOuNt + COUNTROWS(SALES) + COUNTROWS('SaLeS')";
        var expected_o = "VAR XXXXXX = 1 RETURN XXXXXX + COUNTROWS(YYYYY) + COUNTROWS('YYYYY')";
        var expected_d = "VAR AMOUNT = 1 RETURN AMOUNT + COUNTROWS(SALES) + COUNTROWS('SALES')";

        DaxText[] texts =
        [
            new("AMOUNT", "XXXXXX"),
            new("SALES", "YYYYY")
        ];
        var actual_o = CreateObfuscator(texts).ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(texts).DeobfuscateExpression(actual_o);

        Assert.Equal(expected_o, actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_StringLiteralEmpty_IsNotObfuscated()
    {
        var expression = """ IF("" = "", "") """;

        var obfuscator = CreateObfuscator();
        var actual_o = obfuscator.ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(obfuscator.Texts).DeobfuscateExpression(actual_o);

        Assert.Empty(obfuscator.Texts);
        Assert.Equal(expression, actual_o);
        Assert.Equal(expression, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_StringLiteralWhiteSpace_IsNotObfuscated()
    {
        var expression = """ IF(" " = "  ", "   ") """;

        var obfuscator = CreateObfuscator();
        var actual_o = obfuscator.ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(obfuscator.Texts).DeobfuscateExpression(actual_o);

        Assert.Empty(obfuscator.Texts);
        Assert.Equal(expression, actual_o);
        Assert.Equal(expression, actual_d);
    }

    [Fact]
    public void ObfuscateExpression_StringLiteralWithEscapedQuotationMark_IsObfuscated()
    {
        var expression = """"" """" """"";
        var expected_o = """"" "X" """"";
        var expected_d = expression;

        var text = new DaxText("\"", "X");
        var obfuscator = CreateObfuscator([text]);
        var actual_o = obfuscator.ObfuscateExpression(expression);
        var actual_d = CreateDeobfuscator(obfuscator.Texts).DeobfuscateExpression(actual_o);

        Assert.Single(obfuscator.Texts);
        Assert.Contains(text, obfuscator.Texts, DaxTextEqualityComparer.Instance);
        Assert.Equal(expected_o, actual_o);
        Assert.Equal(expected_d, actual_d);
    }

    [Theory]
    [InlineData(nameof(DaxToken.DISPLAYFOLDER))]
    [InlineData(nameof(DaxToken.FORMATSTRING))]
    [InlineData(nameof(DaxToken.DESCRIPTION))] // <- not a valid variable name but it's fine for the test
    [InlineData(nameof(DaxToken.VISIBLE))] // <----- not a valid variable name but it's fine for the test
    [InlineData(nameof(DaxToken.DATATYPE))]
    public void ObfuscateExpression_DDLDaxKeyword_IsNotObfuscated(string name)
    {
        var expression = $""" VAR {name} = 0 RETURN {name} """;
        var actual = CreateObfuscator().ObfuscateExpression(expression);

        Assert.Equal(expression, actual);
    }

    [Theory]
    [InlineData("Amount")]
    [InlineData("Sales Amount")]
    [InlineData("__myvar")]
    public void ObfuscateText_ReobfuscatingObfuscatedText_DoesNotChangeObfuscatedValue(string value)
    {
        var obfuscator = CreateObfuscator();
        var text = new DaxText(value);
        var expected = obfuscator.ObfuscateText(new DaxText(value));

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
        var obfuscator = CreateObfuscator();

        foreach (var value in values)
        {
            var text = new DaxText(value);
            _ = obfuscator.ObfuscateText(text); // << this throws in case of unresolved conflict
        }

        Assert.Equal(values.Count, obfuscator.Texts.Count);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("   ")]
    public void ObfuscateText_WhiteSpaceValue_IsNotObfuscated(string value)
    {
        var obfuscator = CreateObfuscator();
        var text = new DaxText(value);
        _ = obfuscator.ObfuscateText(text);

        AssertThat.IsNotObfuscated(text);
        Assert.DoesNotContain(text, obfuscator.Texts, DaxTextValueEqualityComparer.Instance);
    }

    [Fact]
    public void ObfuscateText_SingleCharLength_IsExtended()
    {
        var obfuscator = CreateObfuscator();

        // Seed the dictionary
        foreach (var @char in "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
            obfuscator.Texts.Add(new DaxText(@char.ToString(), @char.ToString()));

        // Here the obfuscator starts extending the obfuscated value length to resolve conflicts
        foreach (var @char in "0123456789=1£$%&(){+")
        {
            var value = obfuscator.ObfuscateText(new DaxText(@char.ToString()));
            Assert.True(value.Length > 1);
        }
    }

    [DebuggerStepThrough]
    private static DaxModelObfuscator CreateObfuscator() => CreateObfuscator([]);

    [DebuggerStepThrough]
    private static DaxModelObfuscator CreateObfuscator(DaxText[] texts, Model? model = null)
    {
        var obfuscator = new DaxModelObfuscator(model ?? new Model());
        foreach (var text in texts)
            obfuscator.Texts.Add(text);
        return obfuscator;
    }

    [DebuggerStepThrough]
    private static DaxModelDeobfuscator CreateDeobfuscator(IEnumerable<DaxText> texts)
    {
        var obfuscationTexts = texts.Select((t) => t.ToObfuscationText()).ToArray();
        var dictionary = new ObfuscationDictionary(id: Guid.NewGuid().ToString("D"), "0.0.0-test", obfuscationTexts);
        return CreateDeobfuscator(dictionary);
    }

    [DebuggerStepThrough]
    private static DaxModelDeobfuscator CreateDeobfuscator(ObfuscationDictionary dictionary, Model? model = null)
    {
        if (model == null)
        {
            model = new();
            model.ObfuscatorDictionaryId = dictionary.Id;
        }
        return new DaxModelDeobfuscator(model, dictionary);
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

    public static TheoryData<string> GetDaxKeywordData()
    {
        var data = new TheoryData<string>();
        foreach (var kw in Constants.DaxKeywords)
            data.Add(kw);
        return data;
    }

    public static TheoryData<string> GetDaxReservedNameData() => new()
    {
        { Constants.DaxKeyword_Date },
        { Constants.DaxKeyword_Value },
        // Include table constructor { } [ValueN] names that are not in the list
        { Constants.DaxKeyword_Value + "1" },
        { Constants.DaxKeyword_Value + "2" },
    };
}
