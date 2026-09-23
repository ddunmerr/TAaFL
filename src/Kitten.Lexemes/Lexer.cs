using System.Globalization;
using System.Text;

namespace Kitten.Lexemes;

/// <summary>
/// Лексический анализатор языка Kitten.
/// Разбирает исходный код посимвольно и возвращает лексемы по одной.
/// </summary>
public class Lexer
{
    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        { "bool", TokenType.Bool },
        { "true", TokenType.True },
        { "false", TokenType.False },
        { "if", TokenType.If },
        { "else", TokenType.Else },
        { "for", TokenType.For },
        { "while", TokenType.While },
        { "func", TokenType.Func },
        { "var", TokenType.Var },
        { "int", TokenType.Int },
        { "string", TokenType.String },
        { "return", TokenType.Return },
        { "struct", TokenType.Struct },
        { "void", TokenType.Void },
    };

    private static readonly Dictionary<char, char> Escapes = new()
    {
        { 'n', '\n' },
        { '"', '\"' },
        { '\\', '\\' },
    };

    private readonly TextScanner _scanner;

    public Lexer(string code)
    {
        _scanner = new TextScanner(code);
    }

    public Token ParseToken()
    {
        SkipWhiteSpacesAndComments();

        if (_scanner.IsEnd())
        {
            return new Token(TokenType.EndOfFile);
        }

        char c = _scanner.Peek();
        if (char.IsAsciiLetter(c))
        {
            return ParseIdentifierOrKeyWord();
        }

        if (char.IsAsciiDigit(c))
        {
            return ParseIntLiteral();
        }

        if (c == '"')
        {
            return ParseStringLiteral();
        }

        switch (c)
        {
            case '+':
                _scanner.Advance();
                return new Token(TokenType.Plus);

            case '-':
                _scanner.Advance();
                if (_scanner.Peek() == '>')
                {
                    _scanner.Advance();
                    return new Token(TokenType.Arrow);
                }

                return new Token(TokenType.Minus);

            case '*':
                _scanner.Advance();
                return new Token(TokenType.Multiply);

            case '/':
                _scanner.Advance();
                return new Token(TokenType.Divide);

            case '=':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.Equal);
                }

                return new Token(TokenType.Assign);

            case '<':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.LessThanOrEqual);
                }

                return new Token(TokenType.LessThan);

            case '>':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.GreaterThanOrEqual);
                }

                return new Token(TokenType.GreaterThan);

            case '!':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.NotEqual);
                }

                return new Token(TokenType.Not);

            case '&':
                _scanner.Advance();
                return new Token(TokenType.And);

            case '|':
                _scanner.Advance();
                return new Token(TokenType.Or);

            case '.':
                _scanner.Advance();
                return new Token(TokenType.Dot);

            case ',':
                _scanner.Advance();
                return new Token(TokenType.Comma);

            case ':':
                _scanner.Advance();
                return new Token(TokenType.Colon);

            case ';':
                _scanner.Advance();
                return new Token(TokenType.Semicolon);

            case '(':
                _scanner.Advance();
                return new Token(TokenType.OpenParenthesis);

            case ')':
                _scanner.Advance();
                return new Token(TokenType.CloseParenthesis);

            case '[':
                _scanner.Advance();
                return new Token(TokenType.OpenBracket);

            case ']':
                _scanner.Advance();
                return new Token(TokenType.CloseBracket);

            case '{':
                _scanner.Advance();
                return new Token(TokenType.OpenBrace);

            case '}':
                _scanner.Advance();
                return new Token(TokenType.CloseBrace);
        }

        _scanner.Advance();
        return new Token(TokenType.Error, c.ToString());
    }

    /// <summary>
    /// Разбирает литерал целого числа. Возвращает лексему Error, если число выходит за пределы типа данных int.
    /// </summary>
    private Token ParseIntLiteral()
    {
        // NOTE: Сохраняем цифры в буфер, чтобы не терять данные при возврате лексемы Error.
        StringBuilder sb = new();
        for (char ch = _scanner.Peek(); char.IsAsciiDigit(ch); ch = _scanner.Peek())
        {
            sb.Append(ch);
            _scanner.Advance();
        }

        string digits = sb.ToString();

        if (digits.Length > 1 && digits[0] == '0')
        {
            return new Token(TokenType.Error, digits);
        }

        if (int.TryParse(digits, CultureInfo.InvariantCulture, out int value))
        {
            return new Token(TokenType.IntLiteral, value);
        }

        return new Token(TokenType.Error, digits);
    }

    /// <summary>
    /// Разбирает литерал строки.
    /// Возвращает лексему Error, если:
    ///   1) встречает неизвестную escape-последовательность
    ///   2) у строки нет закрывающей кавычки
    /// </summary>
    private Token ParseStringLiteral()
    {
        StringBuilder valueBuilder = new();
        bool hasError = false;

        // Пропускаем открывающую кавычку.
        _scanner.Advance();

        for (char c = _scanner.Peek(); c != '"'; c = _scanner.Peek())
        {
            if (_scanner.IsEnd())
            {
                return new Token(TokenType.Error, valueBuilder.ToString());
            }

            if (c == '\\')
            {
                if (!DecodeEscapeSequence(valueBuilder))
                {
                    hasError = true;
                    valueBuilder.Append('\\');
                }
            }
            else
            {
                valueBuilder.Append(c);
                _scanner.Advance();
            }
        }

        // Пропускаем закрывающую кавычку.
        _scanner.Advance();

        if (hasError)
        {
            return new Token(TokenType.Error, valueBuilder.ToString());
        }

        return new Token(TokenType.StringLiteral, valueBuilder.ToString());
    }

    private bool DecodeEscapeSequence(StringBuilder valueBuilder)
    {
        // Предполагаем, что первый символ — обратный слеш "\".
        _scanner.Advance();

        char ch1 = _scanner.Peek();

        // Разбор простой escape-последовательности: "\n", "\"" и так далее.
        if (Escapes.TryGetValue(ch1, out char unescaped1))
        {
            _scanner.Advance();
            valueBuilder.Append(unescaped1);
            return true;
        }

        return false;
    }

    /// <summary>
    ///  Распознаёт идентификаторы.
    ///  Правила:
    ///     identifier = letter, { letter | digit | "_" } ;
    ///     letter = a..z | A..Z ;
    ///     digit = 0..9 ;
    /// </summary>
    private Token ParseIdentifierOrKeyWord()
    {
        string value = _scanner.Peek().ToString();
        _scanner.Advance();

        for (char c = _scanner.Peek(); char.IsAsciiLetter(c) || c == '_' || char.IsAsciiDigit(c); c = _scanner.Peek())
        {
            value += c;
            _scanner.Advance();
        }

        // Проверяем на совпадение с ключевым словом (с учётом регистра).
        if (Keywords.TryGetValue(value, out TokenType type))
        {
            return new Token(type);
        }

        // Возвращаем токен идентификатора.
        return new Token(TokenType.Identifier, value);
    }

    /// <summary>
    /// Пропускает пробельные символы и комментарии, пока не встретит лексему.
    /// </summary>
    private void SkipWhiteSpacesAndComments()
    {
        do
        {
            SkipWhiteSpaces();
        }
        while (SkipComment());
    }

    /// <summary>
    ///  Пропускает пробельные символы, пока не встретит иной символ.
    /// </summary>
    private void SkipWhiteSpaces()
    {
        while (char.IsWhiteSpace(_scanner.Peek()))
        {
            _scanner.Advance();
        }
    }

    /// <summary>
    /// Пропускает комментарии.
    /// </summary>
    private bool SkipComment()
    {
        /* блочный */
        if (_scanner.Peek() == '/' && _scanner.Peek(1) == '*')
        {
            _scanner.Advance(); // Пропускаем '/'.
            _scanner.Advance(); // Пропускаем '*'.

            while (!_scanner.IsEnd())
            {
                if (_scanner.Peek() == '*' && _scanner.Peek(1) == '/')
                {
                    // Встретили конец комментария.
                    break;
                }

                // Пропускаем вложенный комментарий либо один последующий символ.
                if (!SkipComment())
                {
                    _scanner.Advance();
                }
            }

            _scanner.Advance(); // Пропускаем '*'.
            _scanner.Advance(); // Пропускаем '/'.
            return true;
        }

        // однострочный
        if (_scanner.Peek() == '/' && _scanner.Peek(1) == '/')
        {
            _scanner.Advance(); // Пропускаем первый '/'.
            _scanner.Advance(); // Пропускаем второй '/'.

            while (!_scanner.IsEnd() &&
                   _scanner.Peek() != '\n' &&
                   _scanner.Peek() != '\r')
            {
                _scanner.Advance();
            }

            return true;
        }

        return false;
    }
}