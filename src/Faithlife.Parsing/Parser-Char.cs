using System;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		public static IParser<char> Char(Func<char, bool> test)
		{
			return Parser.Create(input =>
			{
				if (!input.AtEnd)
				{
					char current = input.Current;
					if (test(current))
						return Result.Success(current, input.Advance());
				}

				return Result.Failure<char>(input);
			});
		}

		public static IParser<char> Char(char ch)
		{
			return Parser.Char(x => x == ch);
		}

		public static IParser<char> AnyChar = Parser.Char(x => true);

		public static IParser<char> AnyCharExcept(char ch)
		{
			return Parser.Char(x => x != ch);
		}

		public static IParser<char> Digit = Parser.Char(char.IsDigit);

		public static IParser<char> Letter = Parser.Char(char.IsLetter);

		public static IParser<char> LetterOrDigit = Parser.Char(char.IsLetterOrDigit);

		public static IParser<char> WhiteSpace = Parser.Char(char.IsWhiteSpace);
	}
}
