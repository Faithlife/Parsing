using System;
using System.Collections.Generic;
using System.Linq;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		public static IParser<string> String(string text)
		{
			return Parser.Create(input =>
			{
				int inputIndex = input.Index;
				int textLength = text.Length;
				if (string.CompareOrdinal(input.Text, inputIndex, text, 0, textLength) == 0)
					return Result.Success(text, input.Advance(textLength));

				return Result.Failure<string>(input);
			});
		}

		public static IParser<string> StringIgnoreCase(string text)
		{
			return Parser.Create(input =>
			{
				string inputText = input.Text;
				int inputIndex = input.Index;
				int textLength = text.Length;
				if (string.Compare(inputText, inputIndex, text, 0, textLength, StringComparison.OrdinalIgnoreCase) == 0)
					return Result.Success(inputText.Substring(inputIndex, textLength), input.Advance(textLength));

				return Result.Failure<string>(input);
			});
		}

		public static IParser<IReadOnlyList<char>> Chars(this IParser<string> textParser)
		{
			return textParser.Select(text => text.ToCharArray());
		}

		public static IParser<string> String(this IParser<IEnumerable<char>> parser)
		{
			return parser.Select(chars => new string(chars.ToArray()));
		}

		public static IParser<string> Join(this IParser<IEnumerable<string>> parser, string separator)
		{
			return parser.Select(strings => string.Join(separator, strings.ToArray()));
		}
	}
}
