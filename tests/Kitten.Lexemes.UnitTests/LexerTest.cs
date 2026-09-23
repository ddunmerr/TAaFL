namespace Kitten.Lexemes.UnitTests;

public class LexerTest
{
    [Theory]
    [MemberData(nameof(GetTokenizeIdentifiersAndKeywordsData))]
    [MemberData(nameof(GetTokenizeLiteralsData))]
    [MemberData(nameof(GetSkipWhitespacesAndCommentsData))]
    [MemberData(nameof(GetTokenizePunctuationData))]
    public void Can_tokenize_lexemes(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        Assert.Equal(expected.Count, actual.Count);
        for (int i = 0, iEnd = actual.Count; i < iEnd; ++i)
        {
            Assert.Equal(expected[i], actual[i]);
        }
    }

    public static TheoryData<string, List<Token>> GetTokenizeIdentifiersAndKeywordsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "alice Bob C00L D_2 x", [
                    new Token(TokenType.Identifier, "alice"),
                    new Token(TokenType.Identifier, "Bob"),
                    new Token(TokenType.Identifier, "C00L"),
                    new Token(TokenType.Identifier, "D_2"),
                    new Token(TokenType.Identifier, "x")
                ]
            },
            {
                "var x: int = 0", [
                    new Token(TokenType.Var),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Colon),
                    new Token(TokenType.Int),
                    new Token(TokenType.Assign),
                    new Token(TokenType.IntLiteral, 0),
                ]
            },
            {
                "func foo() -> int { return 0 }", [
                    new Token(TokenType.Func),
                    new Token(TokenType.Identifier, "foo"),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.CloseParenthesis),
                    new Token(TokenType.Arrow),
                    new Token(TokenType.Int),
                    new Token(TokenType.OpenBrace),
                    new Token(TokenType.Return),
                    new Token(TokenType.IntLiteral, 0),
                    new Token(TokenType.CloseBrace),
                ]
            },
            {
                "struct Point { x: int, y: int }", [
                    new Token(TokenType.Struct),
                    new Token(TokenType.Identifier, "Point"),
                    new Token(TokenType.OpenBrace),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Colon),
                    new Token(TokenType.Int),
                    new Token(TokenType.Comma),
                    new Token(TokenType.Identifier, "y"),
                    new Token(TokenType.Colon),
                    new Token(TokenType.Int),
                    new Token(TokenType.CloseBrace),
                ]
            },
            {
                "bool flag = true", [
                    new Token(TokenType.Bool),
                    new Token(TokenType.Identifier, "flag"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.True),
                ]
            },
            {
                "false", [
                    new Token(TokenType.False),
                ]
            },
            {
                "if x > 0 { return 1 } else { return 0 }", [
                    new Token(TokenType.If),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.GreaterThan),
                    new Token(TokenType.IntLiteral, 0),
                    new Token(TokenType.OpenBrace),
                    new Token(TokenType.Return),
                    new Token(TokenType.IntLiteral, 1),
                    new Token(TokenType.CloseBrace),
                    new Token(TokenType.Else),
                    new Token(TokenType.OpenBrace),
                    new Token(TokenType.Return),
                    new Token(TokenType.IntLiteral, 0),
                    new Token(TokenType.CloseBrace),
                ]
            },
            {
                "while x < 10 { x = x + 1 }", [
                    new Token(TokenType.While),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.LessThan),
                    new Token(TokenType.IntLiteral, 10),
                    new Token(TokenType.OpenBrace),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Plus),
                    new Token(TokenType.IntLiteral, 1),
                    new Token(TokenType.CloseBrace),
                ]
            },
            {
                "for (i = 0; i < n; i = i + 1) { print(i) }", [
                    new Token(TokenType.For),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, "i"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.IntLiteral, 0),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.Identifier, "i"),
                    new Token(TokenType.LessThan),
                    new Token(TokenType.Identifier, "n"),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.Identifier, "i"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.Identifier, "i"),
                    new Token(TokenType.Plus),
                    new Token(TokenType.IntLiteral, 1),
                    new Token(TokenType.CloseParenthesis),
                    new Token(TokenType.OpenBrace),
                    new Token(TokenType.Identifier, "print"),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, "i"),
                    new Token(TokenType.CloseParenthesis),
                    new Token(TokenType.CloseBrace),
                ]
            },
            {
                "string name = \"\"", [
                    new Token(TokenType.String),
                    new Token(TokenType.Identifier, "name"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.StringLiteral, ""),
                ]
            },
            {
                "func hello() -> void { }", [
                    new Token(TokenType.Func),
                    new Token(TokenType.Identifier, "hello"),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.CloseParenthesis),
                    new Token(TokenType.Arrow),
                    new Token(TokenType.Void),
                    new Token(TokenType.OpenBrace),
                    new Token(TokenType.CloseBrace),
                ]
            },
            {
                "IF", [
                    new Token(TokenType.Identifier, "IF"),
                ]
            },
            {
                "int _x", [
                    new Token(TokenType.Int),
                    new Token(TokenType.Error, "_"),
                    new Token(TokenType.Identifier, "x"),
                    ]
            },
            {
                "int x_", [
                    new Token(TokenType.Int),
                    new Token(TokenType.Identifier, "x_"),
                    ]
            },
            {
                "int x_y", [
                    new Token(TokenType.Int),
                    new Token(TokenType.Identifier, "x_y"),
                    ]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetTokenizeLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "0 1234 56789", [
                    new Token(TokenType.IntLiteral, 0),
                    new Token(TokenType.IntLiteral, 1234),
                    new Token(TokenType.IntLiteral, 56789),
                ]
            },
            {
                """
                "" "0" "Hello, world!"
                """,
                [
                    new Token(TokenType.StringLiteral, ""),
                    new Token(TokenType.StringLiteral, "0"),
                    new Token(TokenType.StringLiteral, "Hello, world!"),
                ]
            },
            {
                // Разбор простых escape-последовательностей
                """
                "\n\"\\"
                """,
                [
                    new Token(TokenType.StringLiteral, "\n\"\\"),
                ]
            },
            {
                 // Неизвестная escape-последовательность.
                """
                "\x"
                """,
                [
                    new Token(TokenType.Error, "\\x"),
                ]
            },
            {
                // Максимум int.
                "2147483647", [
                    new Token(TokenType.IntLiteral, 2147483647),
                ]
            },
            {
                // Переполнение int.
                "2147483648", [
                    new Token(TokenType.Error, "2147483648"),
                ]
            },
            {
                // Ведущий ноль.
                "007", [
                    new Token(TokenType.Error, "007"),
                ]
            },
            {
                // Отрицательное число - это унарный минус и положительный литерал.
                "-42", [
                    new Token(TokenType.Minus),
                    new Token(TokenType.IntLiteral, 42),
                ]
            },
            {
                // Минус перед нулем.
                "-0", [
                    new Token(TokenType.Minus),
                    new Token(TokenType.IntLiteral, 0),
                ]
            },
            {
                // Минус перед максимумом int.
                "-2147483647", [
                    new Token(TokenType.Minus),
                    new Token(TokenType.IntLiteral, 2147483647),
                ]
            },
            {
                // Незакрытая строка.
                """
                "abc
                """,
                [
                    new Token(TokenType.Error, "abc"),
                ]
            },
            {
                "@", [ new Token(TokenType.Error, "@")]
            },
            {
                "#", [ new Token(TokenType.Error, "#")]
            },
            {
                "$", [ new Token(TokenType.Error, "$")]
            },
            {
                "Я", [ new Token(TokenType.Error, "Я")]
            },
            {
                "✈️", [new Token(TokenType.Error, "\u2708"),
                       new Token(TokenType.Error, "\ufe0f")]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetSkipWhitespacesAndCommentsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                // Пропуск пробельных символов.
                "x \t\r\n\fy", [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Identifier, "y"),
                ]
            },
            {
                // Пропуск комментариев.
                "/* comments */ a / /* should be */ b * c /* ignored */ ", [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Divide),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.Multiply),
                    new Token(TokenType.Identifier, "c"),
                ]
            },
            {
                // Пропуск вложенных комментариев.
                "a / b /* nested /* comments */ are allowed */ * c", [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Divide),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.Multiply),
                    new Token(TokenType.Identifier, "c"),
                ]
            },
            {
                // Однострочный комментарий.
                "a // comment\nb", [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Identifier, "b"),
                ]
            },
            {
                // Пустой ввод
                "", []
            },
            {
                // Только пробельные символы
                "   \t\n", []
            },
        };
    }

    public static TheoryData<string, List<Token>> GetTokenizePunctuationData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "x + y / (10 - z * 2)", [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Plus),
                    new Token(TokenType.Identifier, "y"),
                    new Token(TokenType.Divide),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.IntLiteral, 10),
                    new Token(TokenType.Minus),
                    new Token(TokenType.Identifier, "z"),
                    new Token(TokenType.Multiply),
                    new Token(TokenType.IntLiteral, 2),
                    new Token(TokenType.CloseParenthesis),
                ]
            },
            {
                "a < b | a > c | b = c", [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.LessThan),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.Or),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.GreaterThan),
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.Or),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.Identifier, "c"),
                ]
            },
            {
                "a <= b & b >= c & a == c", [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.LessThanOrEqual),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.And),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.GreaterThanOrEqual),
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.And),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Equal),
                    new Token(TokenType.Identifier, "c"),
                ]
            },
            {
                "point.x = v[0]", [
                    new Token(TokenType.Identifier, "point"),
                    new Token(TokenType.Dot),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.Identifier, "v"),
                    new Token(TokenType.OpenBracket),
                    new Token(TokenType.IntLiteral, 0),
                    new Token(TokenType.CloseBracket),
                ]
            },
            {
                "struct Point { x: int, y: int }", [
                    new Token(TokenType.Struct),
                    new Token(TokenType.Identifier, "Point"),
                    new Token(TokenType.OpenBrace),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Colon),
                    new Token(TokenType.Int),
                    new Token(TokenType.Comma),
                    new Token(TokenType.Identifier, "y"),
                    new Token(TokenType.Colon),
                    new Token(TokenType.Int),
                    new Token(TokenType.CloseBrace),
                ]
            },
            {
                "(foo(x);0)", [
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, "foo"),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.CloseParenthesis),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.IntLiteral, 0),
                    new Token(TokenType.CloseParenthesis),
                ]
            },
            {
                "a != b", [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.NotEqual),
                    new Token(TokenType.Identifier, "b"),
                ]
            },
            {
                "!flag", [
                    new Token(TokenType.Not),
                    new Token(TokenType.Identifier, "flag"),
                ]
            },
            {
                "->", [
                    new Token(TokenType.Arrow),
                ]
            },
            {
                "-->", [
                    new Token(TokenType.Minus),
                    new Token(TokenType.Arrow),
                ]
            },
        };
    }

    private static List<Token> Tokenize(string code)
    {
        List<Token> results = [];
        Lexer lexer = new(code);
        for (Token t = lexer.ParseToken(); t.Type != TokenType.EndOfFile; t = lexer.ParseToken())
        {
            results.Add(t);
        }

        return results;
    }
}