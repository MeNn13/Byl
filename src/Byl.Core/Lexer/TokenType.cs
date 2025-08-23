using Byl.Core.Lexer.Extensions;
using Byl.Core.Lexer.TokenMatcher;

namespace Byl.Core.Lexer;

public enum TokenType
{
    #region Keyworld
    [Keyword("главный")]
    [TokenMatcher(typeof(KeywordMatcher))]
    Main = 1,

    [Keyword("печать")]
    [TokenMatcher(typeof(KeywordMatcher))]
    Print,

    [Keyword("если")] 
    [TokenMatcher(typeof(KeywordMatcher))]
    If,

    [Keyword("иначе")]
    [TokenMatcher(typeof(KeywordMatcher))]
    Else,

    [Keyword("пока")] 
    [TokenMatcher(typeof(KeywordMatcher))]
    While,

    [Keyword("вернуть")]
    [TokenMatcher(typeof(KeywordMatcher))]
    Return,

    [Keyword("этот")]
    [TokenMatcher(typeof(KeywordMatcher))]
    This,

    [Keyword("статик")]
    [TokenMatcher(typeof(KeywordMatcher))]
    Static,

    #endregion

    #region OOP
    [Keyword("пространство")]
    [TokenMatcher(typeof(KeywordMatcher))]
    Namespace,

    [Keyword("класс")]
    [TokenMatcher(typeof(KeywordMatcher))]
    Class,

    [Keyword("новый")]
    [TokenMatcher(typeof(KeywordMatcher))]
    New,

    [Keyword("родитель")]
    [TokenMatcher(typeof(KeywordMatcher))]
    Base,

    [Keyword("Конструктор")]
    [TokenMatcher(typeof(KeywordMatcher))]
    Constructor,

    [Keyword("импорт")]
    [TokenMatcher(typeof(KeywordMatcher))]
    Import,

    [Char(':')]
    Colon,

    [MultiChar("::")]
    NamespaceSeparator,
    #endregion

    #region Modifire of Access
    [Keyword("публичный")]
    [TokenMatcher(typeof(KeywordMatcher))]
    Public,

    [Keyword("приватный")]
    [TokenMatcher(typeof(KeywordMatcher))]
    Private,

    [Keyword("защищенный")]
    [TokenMatcher(typeof(KeywordMatcher))]
    Protected,
    #endregion

    #region Type
    [Keyword("общ")]
    [TokenMatcher(typeof(KeywordMatcher))]
    VarType,

    [Keyword("цел")]
    [TokenMatcher(typeof(KeywordMatcher))]
    IntType,

    [Keyword("вещ")]
    [TokenMatcher(typeof(KeywordMatcher))]
    FloatType,

    [Keyword("стр")] 
    [TokenMatcher(typeof(KeywordMatcher))]
    StringType,

    [Keyword("лог")] 
    [TokenMatcher(typeof(KeywordMatcher))]
    BoolType,

    #endregion

    #region Bool

    [Keyword("истина")]
    [TokenMatcher(typeof(KeywordAttribute))] 
    True,

    [Keyword("ложь")]
    [TokenMatcher(typeof(KeywordAttribute))]
    False,

    #endregion

    #region Logic

    [TokenMatcher(typeof(CharAttribute))]
    [Char('=')] Assign,
    [Char('>')] GreaterThan,
    [Char('<')] LessThan,
    [MultiChar("==")] Equal,
    [MultiChar("!=")] NotEqual,
    [MultiChar("&&")] And,
    [MultiChar("||")] Or,
    [Char('!')] Not,

    #endregion

    #region Math
    [Char('+')] Plus,
    [Char('-')] Minus,
    [Char('*')] Multiply,
    [Char('/')] Divide,

    #endregion

    #region Block
    [MultiChar("%\"")] InterpolatedString,
    [Char('(')] LParen,
    [Char(')')] RParen,
    [Char('{')] LBrace,
    [Char('}')] RBrace,
    [Char(';')] Semicolon,
    [MultiChar("=>")] Arrow,
    [Char(',')] Comma,
    [Char('.')] Dot,
    [MultiChar("//")] Comment,

    #endregion

    Identifier,
    Number,
    StringLiteral,
    EOF
}
