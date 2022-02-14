namespace Faithlife.Parsing;

/// <summary>
/// Helper methods for creating and executing parsers.
/// </summary>
public static partial class Parser
{
	/// <summary>
	/// Attempts to parse the specified text.
	/// </summary>
	public static IParseResult<T> TryParse<T>(this IParser<T> parser, string text) => parser.TryParse(text, 0);

	/// <summary>
	/// Attempts to parse the specified text at the specified start index.
	/// </summary>
	public static IParseResult<T> TryParse<T>(this IParser<T> parser, string text, int startIndex) => parser.TryParse(new TextPosition(text, startIndex));

	/// <summary>
	/// Parses the specified text, throwing <see cref="ParseException" /> on failure.
	/// </summary>
	public static T Parse<T>(this IParser<T> parser, string text) => parser.Parse(text, 0);

	/// <summary>
	/// Parses the specified text at the specified start index, throwing <see cref="ParseException" /> on failure.
	/// </summary>
	public static T Parse<T>(this IParser<T> parser, string text, int startIndex)
	{
		var position = new TextPosition(text, startIndex);
		var value = parser.TryParse(ref position, out var success);
		if (!success)
			throw new ParseException(ParseResult.Failure<T>(position));
		return value;
	}

	/// <summary>
	/// Creates a parser from a delegate.
	/// </summary>
	/// <remarks>For maximum performance, derive from <see cref="Parser{T}" />.</remarks>
	public static IParser<T> Create<T>(Func<TextPosition, IParseResult<T>> parse) => new SimpleParser<T>(parse);

	private sealed class SimpleParser<T> : Parser<T>
	{
		public SimpleParser(Func<TextPosition, IParseResult<T>> parse)
		{
			m_parse = parse;
		}

		public override T TryParse(ref TextPosition position, out bool success)
		{
			var parseResult = m_parse(position);
			position = parseResult.NextPosition;
			success = parseResult.Success;
			return success ? parseResult.Value : default!;
		}

		private readonly Func<TextPosition, IParseResult<T>> m_parse;
	}
}

/// <summary>
/// Base class for parsers.
/// </summary>
public abstract class Parser<T> : IParser<T>
{
	/// <summary>
	/// Parses text.
	/// </summary>
	public IParseResult<T> TryParse(TextPosition position)
	{
		var value = TryParse(ref position, out var success);
		return success ? ParseResult.Success(value, position) : ParseResult.Failure<T>(position);
	}

	/// <summary>
	/// Parses text.
	/// </summary>
	public abstract T TryParse(ref TextPosition position, out bool success);
}
