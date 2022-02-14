namespace Faithlife.Parsing;

public static partial class Parser
{
	/// <summary>
	/// Parses the specified string using ordinal (case-sensitive) comparison.
	/// </summary>
	public static IParser<string> String(string text) => String(text, StringComparison.Ordinal);

	/// <summary>
	/// Parses the specified string using the specified string comparison.
	/// </summary>
	public static IParser<string> String(string text, StringComparison comparison) => new StringParser(text, comparison);

	private sealed class StringParser : Parser<string>
	{
		public StringParser(string text, StringComparison comparison) => (m_text, m_comparison) = (text, comparison);

		public override string TryParse(bool skip, ref TextPosition position, out bool success)
		{
			var inputText = position.Text;
			var inputIndex = position.Index;
			var textLength = m_text.Length;
			if (string.Compare(inputText, inputIndex, m_text, 0, textLength, m_comparison) == 0)
			{
				position = position.WithNextIndex(textLength);
				success = true;
				return m_comparison == StringComparison.Ordinal || skip ? m_text : inputText.Substring(inputIndex, textLength);
			}

			success = false;
			return default!;
		}

		private readonly string m_text;
		private readonly StringComparison m_comparison;
	}

	/// <summary>
	/// Maps a successfully parsed string into a successfully parsed collection of characters.
	/// </summary>
	public static IParser<IReadOnlyList<char>> Chars(this IParser<string> textParser) => textParser.Select(text => text.ToCharArray());

	/// <summary>
	/// Maps a successfully parsed collection of characters into a successfully parsed string.
	/// </summary>
	public static IParser<string> String(this IParser<IEnumerable<char>> parser) => parser.Select(chars => new string(chars.ToArray()));

	/// <summary>
	/// Concatenates the successfully parsed collection of strings into a single successfully parsed string.
	/// </summary>
	public static IParser<string> Concat(this IParser<IEnumerable<string>> parser) => parser.Select(string.Concat);

	/// <summary>
	/// Joins the successfully parsed collection of strings into a single successfully parsed string using the specified separator.
	/// </summary>
	public static IParser<string> Join(this IParser<IEnumerable<string>> parser, string separator) => parser.Select(strings => string.Join(separator, strings));
}
