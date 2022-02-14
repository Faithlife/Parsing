namespace Faithlife.Parsing;

public static partial class Parser
{
	/// <summary>
	/// Succeeds if the parser succeeds. The value is a one-item collection of the successfully parsed item.
	/// </summary>
	public static IParser<IReadOnlyList<T>> Once<T>(this IParser<T> parser) => parser.Repeat(1);

	/// <summary>
	/// Always succeeds. The value is a one-item collection of a single successfully parsed item; otherwise an empty collection.
	/// </summary>
	public static IParser<IReadOnlyList<T>> AtMostOnce<T>(this IParser<T> parser) => parser.AtMost(1);

	/// <summary>
	/// Always succeeds. The value is a collection of at most the specified number of successfully parsed items.
	/// </summary>
	public static IParser<IReadOnlyList<T>> AtMost<T>(this IParser<T> parser, int atMost) => new RepeatParser<T>(parser, 0, atMost);

	/// <summary>
	/// Always succeeds. The value is a collection of as many items as can be successfully parsed.
	/// </summary>
	public static IParser<IReadOnlyList<T>> Many<T>(this IParser<T> parser) => parser.AtLeast(0);

	/// <summary>
	/// Succeeds if the parser succeeds at least once. The value is a collection of as many items as can be successfully parsed.
	/// </summary>
	public static IParser<IReadOnlyList<T>> AtLeastOnce<T>(this IParser<T> parser) => parser.AtLeast(1);

	/// <summary>
	/// Succeeds if the parser succeeds at least the specified number of times. The value is a collection of as many items as can be successfully parsed.
	/// </summary>
	public static IParser<IReadOnlyList<T>> AtLeast<T>(this IParser<T> parser, int atLeast) => new RepeatParser<T>(parser, atLeast, null);

	/// <summary>
	/// Succeeds if the parser succeeds the specified number of times. The value is a collection of the parsed items.
	/// </summary>
	public static IParser<IReadOnlyList<T>> Repeat<T>(this IParser<T> parser, int exactly) => new RepeatParser<T>(parser, exactly, exactly);

	/// <summary>
	/// Succeeds if the parser succeeds the specified number of times. The value is a collection of the parsed items.
	/// </summary>
	public static IParser<IReadOnlyList<T>> Repeat<T>(this IParser<T> parser, int atLeast, int atMost) => new RepeatParser<T>(parser, atLeast, atMost);

	/// <summary>
	/// Succeeds if the specified parser succeeds at least once, requiring and ignoring the specified delimiter between each item.
	/// </summary>
	public static IParser<IReadOnlyList<TValue>> Delimited<TValue, TDelimiter>(this IParser<TValue> parser, IParser<TDelimiter> delimiter) =>
		parser.Once().Then(parser.PrecededBy(delimiter).Many(), (first, rest) => first.Concat(rest).ToList());

	/// <summary>
	/// Succeeds if the specified parser succeeds at least once, requiring and ignoring the specified delimiter between each item,
	/// and allowing a single optional trailing delimiter.
	/// </summary>
	public static IParser<IReadOnlyList<TValue>> DelimitedAllowTrailing<TValue, TDelimiter>(this IParser<TValue> parser, IParser<TDelimiter> delimiter) =>
		parser.Once().Then(parser.PrecededBy(delimiter).Many().FollowedBy(delimiter.OrDefault()), (first, rest) => first.Concat(rest).ToList());

	private sealed class RepeatParser<T> : Parser<IReadOnlyList<T>>
	{
		public RepeatParser(IParser<T> parser, int atLeast, int? atMost) => (m_parser, m_atLeast, m_atMost) = (parser, atLeast, atMost);

		public override IReadOnlyList<T> TryParse(ref TextPosition position, out bool success)
		{
			var hasValue = false;
			T value = default!;
			List<T>? values = null;
			var remainder = position;
			var repeated = 0;

			while (true)
			{
				if (repeated >= m_atMost)
					break;
				var currentPosition = remainder;
				var currentValue = m_parser.TryParse(ref currentPosition, out var currentSuccess);
				if (!currentSuccess || currentPosition == remainder)
					break;
				if (!hasValue)
				{
					value = currentValue;
					hasValue = true;
				}
				else
				{
					values ??= new List<T> { value };
					values.Add(currentValue);
				}
				remainder = currentPosition;
				repeated++;
			}

			if (repeated >= m_atLeast)
			{
				position = remainder;
				success = true;
				return values != null ? values : hasValue ? new[] { value } : Array.Empty<T>();
			}

			success = false;
			return default!;
		}

		private readonly IParser<T> m_parser;
		private readonly int m_atLeast;
		private readonly int? m_atMost;
	}
}
