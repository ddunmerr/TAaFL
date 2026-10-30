using Kitten.Lexemes;

namespace Kitten.Parsing;

/// <summary>
/// Поток токенов с предпросмотром вперёд.
/// Хранит текущий токен для операций Peek.
/// </summary>
public class TokenStream
{
    private readonly Lexer _lexer;
    private Token _current;

    /// <summary>
    /// Создаёт поток токенов для указанного исходного кода.
    /// </summary>
    public TokenStream(string code)
    {
        _lexer = new Lexer(code);
        _current = _lexer.ParseToken();
    }

    /// <summary>
    /// Возвращает текущий токен, не сдвигая позицию.
    /// </summary>
    public Token Peek()
    {
        return _current;
    }

    /// <summary>
    /// Сдвигает поток на один токен вперёд.
    /// </summary>
    public void Advance()
    {
        _current = _lexer.ParseToken();
    }
}