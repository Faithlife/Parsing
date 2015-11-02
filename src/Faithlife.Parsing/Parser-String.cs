using System;
using System.Collections.Generic;
using System.Linq;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		/// <summary>
		/// Parses the specified string using ordinal (case-sensitive) comparison.
		/// </summary>
		public static IParser<string> String(string text)
		{
			return String(text, StringComparison.Ordinal);
		}

		/// <summary>
		/// Parses the specified string using the specified string comparison.
		/// </summary>
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

		/// <summary>
		/// Maps a successfully parsed string into a successfully parsed collection of characters.
		/// </summary>
		public static IParser<IReadOnlyList<char>> Chars(this IParser<string> textParser)
		{
			return textParser.Select(text => text.ToCharArray());
		}

		/// <summary>
		/// Maps a successfully parsed collection of characters into a successfully parsed string.
		/// </summary>
		public static IParser<string> String(this IParser<IEnumerable<char>> parser)
		{
			return parser.Select(chars => new string(chars.ToArray()));
		}

		/// <summary>
		/// Concatenates the successfully parsed collection of strings into a single successfully parsed string.
		/// </summary>
		public static IParser<string> Concat(this IParser<IEnumerable<string>> parser)
		{
			return parser.Select(string.Concat);
		}

		/// <summary>
		/// Joins the successfully parsed collection of strings into a single successfully parsed string using the specified separator.
		/// </summary>
		public static IParser<string> Join(this IParser<IEnumerable<string>> parser, string separator)
		{
			return parser.Select(strings => string.Join(separator, strings));
		}
	}
}
