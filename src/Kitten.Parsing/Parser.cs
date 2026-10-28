using System.Globalization;

using Kitten.Lexemes;

namespace Kitten.Parsing;

/// <summary>
/// Синтаксический анализатор языка Kitten.
/// Разбирает программу методом рекурсивного спуска и выполняет ее по мере разбора.
/// Каждое правило грамматики возвращает значение выражения,
/// а инструкции выполняются сразу.
/// </summary>
public class Parser
{
    private readonly TokenStream _tokens;
    private readonly TextReader _input;
    private readonly TextWriter _output;

    public Parser(string code, TextReader input, TextWriter output)
    {
        _tokens = new TokenStream(code);
        _input = input;
        _output = output;
    }

    /// <summary>
    /// Разбирает и выполняет программу.
    /// Правило:
    ///     program = main_function_declaration, end_of_file ;
    /// </summary>
    public void ParseProgram()
    {
        ParseMainFunctionDeclaration();
        Match(TokenType.EndOfFile);
    }

    /// <summary>
    /// Разбирает объявление функции main.
    /// Правило:
    ///     main_function_declaration = "func", "main", "(", ")", "->", "void", block ;
    /// </summary>
    private void ParseMainFunctionDeclaration()
    {
        Match(TokenType.Func);
        MatchIdentifier("main");
        Match(TokenType.OpenParenthesis);
        Match(TokenType.CloseParenthesis);
        Match(TokenType.Arrow);
        Match(TokenType.Void);
        ParseBlock();
    }

    /// <summary>
    /// Разбирает блок инструкций.
    /// Правило:
    ///     block = "{", { statement }, "}" ;
    /// </summary>
    private void ParseBlock()
    {
        Match(TokenType.OpenBrace);

        while (_tokens.Peek().Type != TokenType.CloseBrace && _tokens.Peek().Type != TokenType.EndOfFile)
        {
            ParseStatement();
        }

        Match(TokenType.CloseBrace);
    }

    /// <summary>
    /// Разбирает инструкцию.
    /// Правило:
    ///     statement = print_statement | expression_statement ;
    /// </summary>
    private void ParseStatement()
    {
        if (IsIdentifier(_tokens.Peek(), "print"))
        {
            ParsePrintStatement();
            return;
        }

        ParseExpressionStatement();
    }

    /// <summary>
    /// Разбирает инструкцию вывода числа.
    /// Правило:
    ///     print_statement = "print", "(", expression, ")", ";" ;
    /// </summary>
    private void ParsePrintStatement()
    {
        MatchIdentifier("print");
        Match(TokenType.OpenParenthesis);
        int value = ParseExpression();
        Match(TokenType.CloseParenthesis);
        Match(TokenType.Semicolon);

        _output.WriteLine(value);
    }

    /// <summary>
    /// Разбирает выражение как инструкцию. Результат вычисления отбрасывается.
    /// Правило:
    ///     expression_statement = expression, ";" ;
    /// </summary>
    private void ParseExpressionStatement()
    {
        ParseExpression();
        Match(TokenType.Semicolon);
    }

    /// <summary>
    /// Разбирает и вычисляет выражение.
    /// Правило:
    ///     expression = additive_expression ;
    /// </summary>
    private int ParseExpression()
    {
        return ParseAdditiveExpression();
    }

    /// <summary>
    /// Разбирает и вычисляет выражение с операторами сложения и вычитания.
    /// Правило:
    ///     additive_expression = multiplicative_expression,
    ///         { ("+" | "-"), multiplicative_expression } ;
    /// </summary>
    private int ParseAdditiveExpression()
    {
        int left = ParseMultiplicativeExpression();

        while (_tokens.Peek().Type == TokenType.Plus
               || _tokens.Peek().Type == TokenType.Minus)
        {
            Token op = _tokens.Peek();
            _tokens.Advance();

            int right = ParseMultiplicativeExpression();

            try
            {
                left = op.Type == TokenType.Plus
                    ? checked(left + right)
                    : checked(left - right);
            }
            catch (OverflowException)
            {
                throw new ParseException("Integer overflow", op);
            }
        }

        return left;
    }

    /// <summary>
    /// Разбирает и вычисляет выражение с операторами умножения и деления.
    /// Правило:
    ///     multiplicative_expression = unary_expression,
    ///         { ("*" | "/"), unary_expression } ;
    /// </summary>
    private int ParseMultiplicativeExpression()
    {
        int left = ParseUnaryExpression();

        while (_tokens.Peek().Type == TokenType.Multiply
               || _tokens.Peek().Type == TokenType.Divide)
        {
            Token op = _tokens.Peek();
            _tokens.Advance();

            int right = ParseUnaryExpression();

            try
            {
                left = op.Type == TokenType.Multiply
                    ? checked(left * right)
                    : left / right;
            }
            catch (DivideByZeroException)
            {
                throw new ParseException("Division by zero", op);
            }
            catch (OverflowException)
            {
                throw new ParseException("Integer overflow", op);
            }
        }

        return left;
    }

    /// <summary>
    /// Разбирает и вычисляет выражение с необязательным унарным минусом.
    /// Правило:
    ///     unary_expression = [ "-" ], primary_expression ;
    /// </summary>
    private int ParseUnaryExpression()
    {
        if (_tokens.Peek().Type == TokenType.Minus)
        {
            Token op = _tokens.Peek();
            _tokens.Advance();
            int operand = ParseUnaryExpression();

            try
            {
                return checked(-operand);
            }
            catch (OverflowException)
            {
                throw new ParseException("Integer overflow", op);
            }
        }

        return ParsePrimaryExpression();
    }

    /// <summary>
    /// Разбирает и вычисляет элементарное выражение.
    /// Правило:
    ///     primary_expression = integer_literal
    ///         | function_call
    ///         | "(", expression, ")" ;
    /// </summary>
    private int ParsePrimaryExpression()
    {
        Token t = _tokens.Peek();

        if (t.Type == TokenType.IntLiteral)
        {
            _tokens.Advance();
            return t.Value!.ToInt();
        }

        if (t.Type == TokenType.OpenParenthesis)
        {
            _tokens.Advance();
            int value = ParseExpression();
            Match(TokenType.CloseParenthesis);
            return value;
        }

        if (t.Type == TokenType.Identifier)
        {
            return ParseFunctionCall();
        }

        throw new ParseException($"Expected expression, got {t}", t);
    }

    /// <summary>
    /// Разбирает и выполняет вызов встроенной функции.
    /// Правило:
    ///     function_call = read_int_call | min_max_call | sqr_call ;
    /// </summary>
    private int ParseFunctionCall()
    {
        Token t = _tokens.Peek();
        string name = t.Value!.ToString();

        switch (name)
        {
            case "readInt":
                {
                    _tokens.Advance();
                    Match(TokenType.OpenParenthesis);
                    Match(TokenType.CloseParenthesis);
                    return ReadInt(t);
                }

            case "min":
            case "max":
                {
                    _tokens.Advance();
                    Match(TokenType.OpenParenthesis);
                    int a = ParseExpression();
                    Match(TokenType.Comma);
                    int b = ParseExpression();
                    Match(TokenType.CloseParenthesis);
                    return name == "min" ? Math.Min(a, b) : Math.Max(a, b);
                }

            case "sqr":
                {
                    _tokens.Advance();
                    Match(TokenType.OpenParenthesis);
                    int x = ParseExpression();
                    Match(TokenType.CloseParenthesis);
                    try
                    {
                        return checked(x * x);
                    }
                    catch (OverflowException)
                    {
                        throw new ParseException("Integer overflow", t);
                    }
                }

            default:
                throw new ParseException($"Unknown function '{name}'", t);
        }
    }

    /// <summary>
    /// Читает одно целое число из стандартного потока ввода.
    /// </summary>
    private int ReadInt(Token token)
    {
        string? line = _input.ReadLine();

        if (line is null)
        {
            throw new ParseException("Unexpected end of input", token);
        }

        if (!int.TryParse(line, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
        {
            throw new ParseException($"Invalid integer input: '{line}'", token);
        }

        return value;
    }

    /// <summary>
    /// Читает ожидаемый токен либо бросает исключение, если встречен иной токен.
    /// </summary>
    private Token Match(TokenType expected)
    {
        Token t = _tokens.Peek();

        if (t.Type != expected)
        {
            throw new ParseException($"Expected {expected}, got {t}", t);
        }

        _tokens.Advance();
        return t;
    }

    /// <summary>
    /// Читает идентификатор с ожидаемым текстом либо бросает исключение.
    /// </summary>
    private Token MatchIdentifier(string name)
    {
        Token t = _tokens.Peek();

        if (!IsIdentifier(t, name))
        {
            throw new ParseException($"Expected '{name}', got {t}", t);
        }

        _tokens.Advance();
        return t;
    }

    /// <summary>
    /// Проверяет, что токен является идентификатором с указанным текстом.
    /// </summary>
    private static bool IsIdentifier(Token token, string name)
    {
        return token.Type == TokenType.Identifier
            && token.Value?.ToString() == name;
    }
}