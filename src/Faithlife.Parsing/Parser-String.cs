using System;
using System.Collections.Generic;
using System.Linq;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		public static IParser<string> String(string text)
		{
			return String(text, StringComparison.Ordinal);
		}

		public static IParser<string> String(string text, StringComparison comparison)
		{
			return Create(position =>
			{
				string inputText = position.Text;
				int inputIndex = position.Index;
				int textLength = text.Length;
				if (string.Compare(inputText, inputIndex, text, 0, textLength, comparison) == 0)
					return ParseResult.Success(inputText.Substring(inputIndex, textLength), position.WithNextIndex(textLength));

				return ParseResult.Failure<string>(position);
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
