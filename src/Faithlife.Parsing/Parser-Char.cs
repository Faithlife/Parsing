namespace Faithlife.Parsing;

public static partial class Parser
{
	/// <summary>
	/// Parses a single character if the specified predicate returns true.
	/// </summary>
	public static IParser<char> Char(Func<char, bool> test) => new FuncCharParser(test);

	private abstract class CharParser : Parser<char>
	{
		public override char TryParse(ref TextPosition position, out bool success)
		{
			if (!position.IsAtEnd())
			{
				var current = position.GetCurrentChar();
				if (IsMatch(current))
				{
					success = true;
					position = position.WithNextIndex();
					return current;
				}
			}

			success = false;
			return default;
		}

		protected abstract bool IsMatch(char ch);
	}

	private sealed class FuncCharParser : CharParser
	{
		public FuncCharParser(Func<char, bool> test) => m_test = test;
		protected override bool IsMatch(char ch) => m_test(ch);
		private readonly Func<char, bool> m_test;
	}

	/// <summary>
	/// Parses the specified character.
	/// </summary>
	public static IParser<char> Char(char ch) => new SpecificCharParser(ch);

	private sealed class SpecificCharParser : CharParser
	{
		public SpecificCharParser(char ch) => m_ch = ch;
		protected override bool IsMatch(char ch) => ch == m_ch;
		private readonly char m_ch;
	}

	/// <summary>
	/// Parses any character; i.e. only fails at the end of the text.
	/// </summary>
	public static readonly IParser<char> AnyChar = new AnyCharParser();

	private sealed class AnyCharParser : CharParser
	{
		protected override bool IsMatch(char ch) => true;
	}

	/// <summary>
	/// Parses any character except the specified character.
	/// </summary>
	public static IParser<char> AnyCharExcept(char ch) => new AnyCharExceptParser(ch);

	private sealed class AnyCharExceptParser : CharParser
	{
		public AnyCharExceptParser(char ch) => m_ch = ch;
		protected override bool IsMatch(char ch) => ch != m_ch;
		private readonly char m_ch;
	}

	/// <summary>
	/// Parses any digit (as determined by System.Char.IsDigit).
	/// </summary>
	public static readonly IParser<char> Digit = new DigitParser();

	private sealed class DigitParser : CharParser
	{
		protected override bool IsMatch(char ch) => char.IsDigit(ch);
	}

	/// <summary>
	/// Parses any letter (as determined by System.Char.IsLetter).
	/// </summary>
	public static readonly IParser<char> Letter = new LetterParser();

	private sealed class LetterParser : CharParser
	{
		protected override bool IsMatch(char ch) => char.IsLetter(ch);
	}

	/// <summary>
	/// Parses any letter or digit (as determined by System.Char.IsLetterOrDigit).
	/// </summary>
	public static readonly IParser<char> LetterOrDigit = new LetterOrDigitParser();

	private sealed class LetterOrDigitParser : CharParser
	{
		protected override bool IsMatch(char ch) => char.IsLetterOrDigit(ch);
	}

	/// <summary>
	/// Parses any whitespace character (as determined by System.Char.IsWhiteSpace).
	/// </summary>
	public static readonly IParser<char> WhiteSpace = new WhiteSpaceParser();

	private sealed class WhiteSpaceParser : CharParser
	{
		protected override bool IsMatch(char ch) => char.IsWhiteSpace(ch);
	}
}
