using Dax.Vpax.Obfuscator.Extensions;
using Xunit;

namespace Dax.Vpax.Obfuscator.Tests.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData(" __t [ c ]", " __t ", " c ")]
    [InlineData(" __t_ [ c ]", " __t_ ", " c ")]
    [InlineData(" __t9 [ c ]", " __t9 ", " c ")]
    [InlineData(" _t [ c ]", " _t ", " c ")]
    [InlineData(" 9t [ c ]", null, " 9t [ c ]")]
    [InlineData(" c ", null, " c ")]
    [InlineData(" t [ c ]", " t ", " c ")]
    [InlineData(" t [c]", " t ", "c")]
    [InlineData("@c", null, "@c")]
    [InlineData("@t[\"\"]", null, "@t[\"\"]")]
    [InlineData("@t[c]", null, "@t[c]")]
    [InlineData("'t''2 [%]'[ '\"\" [%]] ]", "'t''2 [%]'", " '\"\" [%]] ")]
    [InlineData("'t [%]'[ c '\"\" [%]] ]", "'t [%]'", " c '\"\" [%]] ")]
    [InlineData("'t [%]''[ c '\"\" [%]] ]", null, "'t [%]''[ c '\"\" [%]] ]")]
    [InlineData("t[\"\"\"\"]", "t", "\"\"\"\"")]
    [InlineData("t[\"\"]", "t", "\"\"")]
    [InlineData("t[c]", "t", "c")]
    [InlineData("'t'[c]", "'t'", "c")]
    [InlineData("'t''[c]", null, "'t''[c]")]
    [InlineData("t[c]]", null, "t[c]]")]
    [InlineData("t[c]]]", "t", "c]]")]
    [InlineData("'t''2 [%]'['c']", "'t''2 [%]'", "'c'")]
    [InlineData("'c'[c1'']'[c2]", null, "'c'[c1'']'[c2]")]
    public void TryGetTableAndColumnNames(string value, string? expectedTable, string expectedColumn)
    {
        _ = StringExtensions.TryGetTableAndColumnNames(value, out var table, out var column);
        Assert.Equal(expectedTable, table);
        Assert.Equal(expectedColumn, column);
    }
}
