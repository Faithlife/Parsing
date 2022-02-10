namespace Faithlife.Parsing;

/// <summary>
/// Helper methods for creating and executing parsers.
/// </summary>
public static partial class Parser
{
	/// <summary>
	/// Creates a parser from a delegate.
	/// </summary>
	public static IParser<T> Create<T>(Func<TextPosition, IParseResult<T>> parse) => new SimpleParser<T>(parse);

	/// <summary>
	/// Attempts to parse the specified text.
	/// </summary>
	public static IParseResult<T> TryParse<T>(this IParser<T> parser, string text) => parser.TryParse(text, 0);

	/// <summary>
	/// Attempts to parse the specified text at the specified start index.
	/// </summary>
	public static IParseResult<T> TryParse<T>(this IParser<T> parser, string text, int startIndex) => parser.TryParse(new TextPosition(text, startIndex));

	/// <summary>
	/// Parses the specified text, throwing ParseException on failure.
	/// </summary>
	public static T Parse<T>(this IParser<T> parser, string text) => parser.Parse(text, 0);

	/// <summary>
	/// Parses the specified text at the specified start index, throwing ParseException on failure.
	/// </summary>
	public static T Parse<T>(this IParser<T> parser, string text, int startIndex) => parser.TryParse(text, startIndex).Value;

	private sealed class SimpleParser<T> : IParser<T>
	{
		public SimpleParser(Func<TextPosition, IParseResult<T>> parse)
		{
			m_parse = parse;
		}

		public IParseResult<T> TryParse(TextPosition position) => m_parse(position);

		private readonly Func<TextPosition, IParseResult<T>> m_parse;
	}
}
