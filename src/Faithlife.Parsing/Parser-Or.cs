namespace Faithlife.Parsing;

public static partial class Parser
{
	/// <summary>
	/// Succeeds with a successful parser, if any.
	/// </summary>
	/// <remarks>The first successful parser that advances the text position is returned.
	/// Otherwise, the first successful parser that does not advance the text position is returned.
	/// Otherwise, the failure that advanced the text position farthest is returned.</remarks>
	public static IParser<T> Or<T>(IEnumerable<IParser<T>> parsers) => new OrParser<T>(parsers);

	private sealed class OrParser<T> : Parser<T>
	{
		public OrParser(IEnumerable<IParser<T>> parsers) => m_parsers = parsers.ToArray();

		public override T TryParse(bool skip, ref TextPosition position, out bool success)
		{
			var hasEmptySuccess = false;
			T firstEmptySuccessValue = default!;
			TextPosition? bestFailurePosition = null;

			foreach (var parser in m_parsers)
			{
				var currentPosition = position;
				var currentValue = parser.TryParse(skip, ref currentPosition, out var currentSuccess);
				if (currentSuccess)
				{
					if (currentPosition == position)
					{
						if (!hasEmptySuccess)
						{
							hasEmptySuccess = true;
							firstEmptySuccessValue = currentValue;
						}
					}
					else
					{
						position = currentPosition;
						success = true;
						return currentValue;
					}
				}
				else if (bestFailurePosition is null || currentPosition.Index > bestFailurePosition.Value.Index)
				{
					bestFailurePosition = currentPosition;
				}
			}

			if (hasEmptySuccess)
			{
				success = true;
				return firstEmptySuccessValue;
			}

			if (bestFailurePosition != null)
				position = bestFailurePosition.Value;

			success = false;
			return default!;
		}

		private readonly IParser<T>[] m_parsers;
	}

	/// <summary>
	/// Succeeds with a successful parser, if any.
	/// </summary>
	/// <remarks>The first successful parser that advances the text position is returned.
	/// Otherwise, the first successful parser that does not advance the text position is returned.
	/// Otherwise, the failure that advanced the text position farthest is returned.</remarks>
	public static IParser<T> Or<T>(params IParser<T>[] parsers) => Or((IEnumerable<IParser<T>>) parsers);

	/// <summary>
	/// Succeeds with a successful parser, if any.
	/// </summary>
	/// <remarks>The first successful parser that advances the text position is returned.
	/// Otherwise, the first successful parser that does not advance the text position is returned.
	/// Otherwise, the failure that advanced the text position farthest is returned.</remarks>
	public static IParser<T> Or<T>(this IParser<T> first, IParser<T> second) => Or(new[] { first, second });

	/// <summary>
	/// Succeeds with the default value if the parser fails.
	/// </summary>
	public static IParser<T> OrDefault<T>(this IParser<T> parser) => parser.OrDefault(default!);

	/// <summary>
	/// Succeeds with the specified value if the parser fails.
	/// </summary>
	public static IParser<T> OrDefault<T>(this IParser<T> parser, T value) => parser.Or(Success(value));

	/// <summary>
	/// Succeeds with an empty collection if the parser fails.
	/// </summary>
	public static IParser<IEnumerable<T>> OrEmpty<T>(this IParser<IEnumerable<T>> parser) => parser.OrDefault(Array.Empty<T>());

	/// <summary>
	/// Succeeds with an empty collection if the parser fails.
	/// </summary>
	public static IParser<IReadOnlyCollection<T>> OrEmpty<T>(this IParser<IReadOnlyCollection<T>> parser) => parser.OrDefault(Array.Empty<T>());

	/// <summary>
	/// Succeeds with an empty collection if the parser fails.
	/// </summary>
	public static IParser<IReadOnlyList<T>> OrEmpty<T>(this IParser<IReadOnlyList<T>> parser) => parser.OrDefault(Array.Empty<T>());
}
