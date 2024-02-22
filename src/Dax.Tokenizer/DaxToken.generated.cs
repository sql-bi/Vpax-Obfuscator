namespace TabularEditor.Dax.Tokenizer
{
    public sealed partial class DaxToken
    {
	    /// <summary>https://dax.guide/SINGLE_LINE_COMMENT</summary>
	    public const int SINGLE_LINE_COMMENT=1;
	    /// <summary>https://dax.guide/DELIMITED_COMMENT</summary>
	    public const int DELIMITED_COMMENT=2;
	    /// <summary>https://dax.guide/WHITESPACES</summary>
	    public const int WHITESPACES=3;
	    /// <summary>https://dax.guide/DATATABLE</summary>
	    public const int DATATABLE=4;
	    /// <summary>https://dax.guide/Z__FIRSTKEYWORD__</summary>
	    public const int Z__FIRSTKEYWORD__=5;
	    /// <summary>https://dax.guide/DDL_MEASURE</summary>
	    public const int DDL_MEASURE=6;
	    /// <summary>https://dax.guide/DDL_COLUMN</summary>
	    public const int DDL_COLUMN=7;
	    /// <summary>https://dax.guide/DDL_TABLE</summary>
	    public const int DDL_TABLE=8;
	    /// <summary>https://dax.guide/DDL_CALCGROUP</summary>
	    public const int DDL_CALCGROUP=9;
	    /// <summary>https://dax.guide/DDL_CALCITEM</summary>
	    public const int DDL_CALCITEM=10;
	    /// <summary>https://dax.guide/DETAILROWS</summary>
	    public const int DETAILROWS=11;
	    /// <summary>https://dax.guide/DEFINE</summary>
	    public const int DEFINE=12;
	    /// <summary>https://dax.guide/EVALUATE</summary>
	    public const int EVALUATE=13;
	    /// <summary>https://dax.guide/ORDER</summary>
	    public const int ORDER=14;
	    /// <summary>https://dax.guide/BY</summary>
	    public const int BY=15;
	    /// <summary>https://dax.guide/START</summary>
	    public const int START=16;
	    /// <summary>https://dax.guide/AT</summary>
	    public const int AT=17;
	    /// <summary>https://dax.guide/RETURN</summary>
	    public const int RETURN=18;
	    /// <summary>https://dax.guide/VAR</summary>
	    public const int VAR=19;
	    /// <summary>https://dax.guide/NOT</summary>
	    public const int NOT=20;
	    /// <summary>https://dax.guide/IN</summary>
	    public const int IN=21;
	    /// <summary>https://dax.guide/ASC</summary>
	    public const int ASC=22;
	    /// <summary>https://dax.guide/DESC</summary>
	    public const int DESC=23;
	    /// <summary>https://dax.guide/SKIP_</summary>
	    public const int SKIP_=24;
	    /// <summary>https://dax.guide/DENSE</summary>
	    public const int DENSE=25;
	    /// <summary>https://dax.guide/BLANK</summary>
	    public const int BLANK=26;
	    /// <summary>https://dax.guide/BLANKS</summary>
	    public const int BLANKS=27;
	    /// <summary>https://dax.guide/SECOND</summary>
	    public const int SECOND=28;
	    /// <summary>https://dax.guide/MINUTE</summary>
	    public const int MINUTE=29;
	    /// <summary>https://dax.guide/HOUR</summary>
	    public const int HOUR=30;
	    /// <summary>https://dax.guide/DAY</summary>
	    public const int DAY=31;
	    /// <summary>https://dax.guide/MONTH</summary>
	    public const int MONTH=32;
	    /// <summary>https://dax.guide/QUARTER</summary>
	    public const int QUARTER=33;
	    /// <summary>https://dax.guide/YEAR</summary>
	    public const int YEAR=34;
	    /// <summary>https://dax.guide/WEEK</summary>
	    public const int WEEK=35;
	    /// <summary>https://dax.guide/BOTH</summary>
	    public const int BOTH=36;
	    /// <summary>https://dax.guide/NONE</summary>
	    public const int NONE=37;
	    /// <summary>https://dax.guide/ONEWAY</summary>
	    public const int ONEWAY=38;
	    /// <summary>https://dax.guide/ONEWAYRIGHTFILTERSLEFT</summary>
	    public const int ONEWAYRIGHTFILTERSLEFT=39;
	    /// <summary>https://dax.guide/ONEWAYLEFTFILTERSRIGHT</summary>
	    public const int ONEWAYLEFTFILTERSRIGHT=40;
	    /// <summary>https://dax.guide/CURRENCY</summary>
	    public const int CURRENCY=41;
	    /// <summary>https://dax.guide/INTEGER</summary>
	    public const int INTEGER=42;
	    /// <summary>https://dax.guide/DOUBLE</summary>
	    public const int DOUBLE=43;
	    /// <summary>https://dax.guide/STRING</summary>
	    public const int STRING=44;
	    /// <summary>https://dax.guide/BOOLEAN</summary>
	    public const int BOOLEAN=45;
	    /// <summary>https://dax.guide/DATETIME</summary>
	    public const int DATETIME=46;
	    /// <summary>https://dax.guide/VARIANT</summary>
	    public const int VARIANT=47;
	    /// <summary>https://dax.guide/TEXT</summary>
	    public const int TEXT=48;
	    /// <summary>https://dax.guide/ALPHABETICAL</summary>
	    public const int ALPHABETICAL=49;
	    /// <summary>https://dax.guide/KEEP</summary>
	    public const int KEEP=50;
	    /// <summary>https://dax.guide/FIRST</summary>
	    public const int FIRST=51;
	    /// <summary>https://dax.guide/LAST</summary>
	    public const int LAST=52;
	    /// <summary>https://dax.guide/DEFAULT</summary>
	    public const int DEFAULT=53;
	    /// <summary>https://dax.guide/TRUE</summary>
	    public const int TRUE=54;
	    /// <summary>https://dax.guide/FALSE</summary>
	    public const int FALSE=55;
	    /// <summary>https://dax.guide/ABS</summary>
	    public const int ABS=56;
	    /// <summary>https://dax.guide/REL</summary>
	    public const int REL=57;
	    /// <summary>https://dax.guide/Z__LASTKEYWORD__</summary>
	    public const int Z__LASTKEYWORD__=58;
	    /// <summary>https://dax.guide/Z__FIRSTPROPERTY__</summary>
	    public const int Z__FIRSTPROPERTY__=59;
	    /// <summary>https://dax.guide/DISPLAYFOLDER</summary>
	    public const int DISPLAYFOLDER=60;
	    /// <summary>https://dax.guide/FORMATSTRING</summary>
	    public const int FORMATSTRING=61;
	    /// <summary>https://dax.guide/DESCRIPTION</summary>
	    public const int DESCRIPTION=62;
	    /// <summary>https://dax.guide/VISIBLE</summary>
	    public const int VISIBLE=63;
	    /// <summary>https://dax.guide/DATATYPE</summary>
	    public const int DATATYPE=64;
	    /// <summary>https://dax.guide/KPISTATUSEXPRESSION</summary>
	    public const int KPISTATUSEXPRESSION=65;
	    /// <summary>https://dax.guide/KPISTATUSDESCRIPTION</summary>
	    public const int KPISTATUSDESCRIPTION=66;
	    /// <summary>https://dax.guide/KPISTATUSGRAPHIC</summary>
	    public const int KPISTATUSGRAPHIC=67;
	    /// <summary>https://dax.guide/KPITRENDEXPRESSION</summary>
	    public const int KPITRENDEXPRESSION=68;
	    /// <summary>https://dax.guide/KPITRENDDESCRIPTION</summary>
	    public const int KPITRENDDESCRIPTION=69;
	    /// <summary>https://dax.guide/KPITRENDGRAPHIC</summary>
	    public const int KPITRENDGRAPHIC=70;
	    /// <summary>https://dax.guide/KPITARGETEXPRESSION</summary>
	    public const int KPITARGETEXPRESSION=71;
	    /// <summary>https://dax.guide/KPITARGETDESCRIPTION</summary>
	    public const int KPITARGETDESCRIPTION=72;
	    /// <summary>https://dax.guide/KPITARGETFORMATSTRING</summary>
	    public const int KPITARGETFORMATSTRING=73;
	    /// <summary>https://dax.guide/PRECEDENCE</summary>
	    public const int PRECEDENCE=74;
	    /// <summary>https://dax.guide/ORDINAL</summary>
	    public const int ORDINAL=75;
	    /// <summary>https://dax.guide/Z__LASTPROPERTY__</summary>
	    public const int Z__LASTPROPERTY__=76;
	    /// <summary>https://dax.guide/DATE_LITERAL</summary>
	    public const int DATE_LITERAL=77;
	    /// <summary>https://dax.guide/INTEGER_LITERAL</summary>
	    public const int INTEGER_LITERAL=78;
	    /// <summary>https://dax.guide/REAL_LITERAL</summary>
	    public const int REAL_LITERAL=79;
	    /// <summary>https://dax.guide/STRING_LITERAL</summary>
	    public const int STRING_LITERAL=80;
	    /// <summary>https://dax.guide/TABLE</summary>
	    public const int TABLE=81;
	    /// <summary>https://dax.guide/COLUMN_OR_MEASURE</summary>
	    public const int COLUMN_OR_MEASURE=82;
	    /// <summary>https://dax.guide/TABLE_OR_VARIABLE</summary>
	    public const int TABLE_OR_VARIABLE=83;
	    /// <summary>https://dax.guide/OTHER_IDENTIFIER</summary>
	    public const int OTHER_IDENTIFIER=84;
	    /// <summary>https://dax.guide/OPEN_CURLY</summary>
	    public const int OPEN_CURLY=85;
	    /// <summary>https://dax.guide/CLOSE_CURLY</summary>
	    public const int CLOSE_CURLY=86;
	    /// <summary>https://dax.guide/OPEN_PARENS</summary>
	    public const int OPEN_PARENS=87;
	    /// <summary>https://dax.guide/CLOSE_PARENS</summary>
	    public const int CLOSE_PARENS=88;
	    /// <summary>https://dax.guide/COMMA</summary>
	    public const int COMMA=89;
	    /// <summary>https://dax.guide/Z__FIRSTOPERATOR__</summary>
	    public const int Z__FIRSTOPERATOR__=90;
	    /// <summary>https://dax.guide/PLUS</summary>
	    public const int PLUS=91;
	    /// <summary>https://dax.guide/MINUS</summary>
	    public const int MINUS=92;
	    /// <summary>https://dax.guide/STAR</summary>
	    public const int STAR=93;
	    /// <summary>https://dax.guide/DIV</summary>
	    public const int DIV=94;
	    /// <summary>https://dax.guide/CARET</summary>
	    public const int CARET=95;
	    /// <summary>https://dax.guide/AMP</summary>
	    public const int AMP=96;
	    /// <summary>https://dax.guide/STRICTEQ</summary>
	    public const int STRICTEQ=97;
	    /// <summary>https://dax.guide/EQ</summary>
	    public const int EQ=98;
	    /// <summary>https://dax.guide/LT</summary>
	    public const int LT=99;
	    /// <summary>https://dax.guide/GT</summary>
	    public const int GT=100;
	    /// <summary>https://dax.guide/OP_AND</summary>
	    public const int OP_AND=101;
	    /// <summary>https://dax.guide/OP_OR</summary>
	    public const int OP_OR=102;
	    /// <summary>https://dax.guide/OP_NE</summary>
	    public const int OP_NE=103;
	    /// <summary>https://dax.guide/OP_LE</summary>
	    public const int OP_LE=104;
	    /// <summary>https://dax.guide/OP_GE</summary>
	    public const int OP_GE=105;
	    /// <summary>https://dax.guide/Z__LASTOPERATOR__</summary>
	    public const int Z__LASTOPERATOR__=106;
	    /// <summary>https://dax.guide/DOT</summary>
	    public const int DOT=107;
	    /// <summary>https://dax.guide/UNTERMINATED_STRING</summary>
	    public const int UNTERMINATED_STRING=108;
	    /// <summary>https://dax.guide/UNTERMINATED_COLREF</summary>
	    public const int UNTERMINATED_COLREF=109;
	    /// <summary>https://dax.guide/UNTERMINATED_TABLEREF</summary>
	    public const int UNTERMINATED_TABLEREF=110;
	    /// <summary>https://dax.guide/UNKNOWN_CHAR</summary>
	    public const int UNKNOWN_CHAR=111;
	}
}