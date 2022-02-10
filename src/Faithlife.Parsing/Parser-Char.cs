namespace Faithlife.Parsing;

public static partial class Parser
{
	/// <summary>
	/// Parses a single character if the specified predicate returns true.
	/// </summary>
	public static IParser<char> Char(Func<char, bool> test)
	{
		return Create(position =>
		{
			if (!position.IsAtEnd())
			{
				var current = position.GetCurrentChar();
				if (test(current))
					return ParseResult.Success(current, position.WithNextIndex());
			}

			return ParseResult.Failure<char>(position);
		});
	}

	/// <summary>
	/// Parses the specified character.
	/// </summary>
	public static IParser<char> Char(char ch) => Char(x => x == ch);

	/// <summary>
	/// Parses any character; i.e. only fails at the end of the text.
	/// </summary>
	public static readonly IParser<char> AnyChar = Char(x => true);

	/// <summary>
	/// Parses any character except the specified character.
	/// </summary>
	public static IParser<char> AnyCharExcept(char ch) => Char(x => x != ch);

	/// <summary>
	/// Parses any digit (as determined by System.Char.IsDigit).
	/// </summary>
	public static readonly IParser<char> Digit = Char(char.IsDigit);

	/// <summary>
	/// Parses any letter (as determined by System.Char.IsLetter).
	/// </summary>
	public static readonly IParser<char> Letter = Char(char.IsLetter);

	/// <summary>
	/// Parses any letter or digit (as determined by System.Char.IsLetterOrDigit).
	/// </summary>
	public static readonly IParser<char> LetterOrDigit = Char(char.IsLetterOrDigit);

	/// <summary>
	/// Parses any whitespace character (as determined by System.Char.IsWhiteSpace).
	/// </summary>
	public static readonly IParser<char> WhiteSpace = Char(char.IsWhiteSpace);
}
