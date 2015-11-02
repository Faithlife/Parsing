using System;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		public static IParser<char> Char(Func<char, bool> test)
		{
			return Create(position =>
			{
				if (!position.IsAtEnd())
				{
					char current = position.GetCurrentChar();
					if (test(current))
						return ParseResult.Success(current, position.WithNextIndex());
				}

				return ParseResult.Failure<char>(position);
			});
		}

		public static IParser<char> Char(char ch)
		{
			return Char(x => x == ch);
		}

		public static IParser<char> AnyChar = Char(x => true);

		public static IParser<char> AnyCharExcept(char ch)
		{
			return Char(x => x != ch);
		}

		public static IParser<char> Digit = Char(char.IsDigit);

		public static IParser<char> Letter = Char(char.IsLetter);

		public static IParser<char> LetterOrDigit = Char(char.IsLetterOrDigit);

		public static IParser<char> WhiteSpace = Char(char.IsWhiteSpace);
	}
}
