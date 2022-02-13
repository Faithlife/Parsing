using System.Diagnostics.CodeAnalysis;

namespace Faithlife.Parsing;

public static partial class Parser
{
	/// <summary>
	/// Executes one parser after another.
	/// </summary>
	public static IParser<TAfter> Then<TBefore, TAfter>(this IParser<TBefore> parser, Func<TBefore, IParser<TAfter>> convertValueToNextParser)
	{
		if (parser is null)
			throw new ArgumentNullException(nameof(parser));
		if (convertValueToNextParser is null)
			throw new ArgumentNullException(nameof(convertValueToNextParser));
		return Create(position => parser.TryParse(position).MapSuccess(result => convertValueToNextParser(result.Value).TryParse(result.NextPosition)));
	}

	/// <summary>
	/// Executes one parser after another.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1414:Tuple types in signatures should have element names", Justification = "No better names.")]
	public static IParser<(TBefore1, TBefore2)> Then<TBefore1, TBefore2>(this IParser<TBefore1> parser, IParser<TBefore2> nextParser)
	{
		if (parser is null)
			throw new ArgumentNullException(nameof(parser));
		if (nextParser is null)
			throw new ArgumentNullException(nameof(nextParser));
		return Create(position => parser.TryParse(position).MapSuccess(result => nextParser.TryParse(result.NextPosition).MapSuccess(nextResult => ParseResult.Success((result.Value, nextResult.Value), nextResult.NextPosition))));
	}

	/// <summary>
	/// Executes one parser after another.
	/// </summary>
	public static IParser<TAfter> Then<TBefore1, TBefore2, TAfter>(this IParser<TBefore1> parser, IParser<TBefore2> nextParser, Func<TBefore1, TBefore2, TAfter> combineValues)
	{
		if (parser is null)
			throw new ArgumentNullException(nameof(parser));
		if (nextParser is null)
			throw new ArgumentNullException(nameof(nextParser));
		return new ThenParser<TBefore1, TBefore2, TAfter>(parser, nextParser, combineValues);
	}

	private sealed class ThenParser<TBefore1, TBefore2, TAfter> : IParser<TAfter>
	{
		public ThenParser(IParser<TBefore1> parser, IParser<TBefore2> nextParser, Func<TBefore1, TBefore2, TAfter> combineValues)
		{
			m_parser = parser;
			m_nextParser = nextParser;
			m_combineValues = combineValues;
		}

		public IParseResult<TAfter> TryParse(TextPosition position)
		{
			var result = m_parser.TryParse(position);
			if (!result.Success)
				return ParseResult.Failure<TAfter>(result.NextPosition);

			var nextResult = m_nextParser.TryParse(result.NextPosition);
			if (!nextResult.Success)
				return ParseResult.Failure<TAfter>(nextResult.NextPosition);

			return ParseResult.Success(m_combineValues(result.Value, nextResult.Value), nextResult.NextPosition);
		}

		private readonly IParser<TBefore1> m_parser;
		private readonly IParser<TBefore2> m_nextParser;
		private readonly Func<TBefore1, TBefore2, TAfter> m_combineValues;
	}

	/// <summary>
	/// Executes one parser after another.
	/// </summary>
	public static IParser<T1> ThenSkip<T1, T2>(this IParser<T1> parser, IParser<T2> nextParser) =>
		parser.Then(nextParser, (value, _) => value);

	/// <summary>
	/// Executes one parser after another.
	/// </summary>
	public static IParser<T2> SkipThen<T1, T2>(this IParser<T1> parser, IParser<T2> nextParser) =>
		parser.Then(nextParser, (_, value) => value);

	/// <summary>
	/// Converts any successfully parsed value.
	/// </summary>
	public static IParser<TAfter> Select<TBefore, TAfter>(this IParser<TBefore> parser, Func<TBefore, TAfter> convertValue)
	{
		if (convertValue is null)
			throw new ArgumentNullException(nameof(convertValue));
		return Create(position => parser.TryParse(position).MapSuccess(result => ParseResult.Success(convertValue(result.Value), result.NextPosition)));
	}

	/// <summary>
	/// Succeeds with the specified value if the parser is successful.
	/// </summary>
	public static IParser<TAfter> Success<TBefore, TAfter>(this IParser<TBefore> parser, TAfter value)
	{
		if (parser is null)
			throw new ArgumentNullException(nameof(parser));
		return Create(position => parser.TryParse(position).MapSuccess(result => ParseResult.Success(value, result.NextPosition)));
	}

	/// <summary>
	/// Concatenates the two successfully parsed collections.
	/// </summary>
	public static IParser<IReadOnlyList<T>> Concat<T>(this IParser<IEnumerable<T>> firstParser, IParser<IEnumerable<T>> secondParser) =>
		firstParser.Then(secondParser, (firstValue, secondValue) => firstValue.Concat(secondValue).ToList());

	/// <summary>
	/// Appends a successfully parsed value to the end of a successfully parsed collection.
	/// </summary>
	public static IParser<IReadOnlyList<T>> Append<T>(this IParser<IEnumerable<T>> firstParser, IParser<T> secondParser) =>
		firstParser.Concat(secondParser.Once());

	/// <summary>
	/// Used to support LINQ query syntax.
	/// </summary>
	public static IParser<TAfter> SelectMany<TBefore, TDuring, TAfter>(this IParser<TBefore> parser, Func<TBefore, IParser<TDuring>> selector, Func<TBefore, TDuring, TAfter> projector)
	{
		if (selector is null)
			throw new ArgumentNullException(nameof(selector));
		if (projector is null)
			throw new ArgumentNullException(nameof(projector));
		return parser.Then(t => selector(t).Select(u => projector(t, u)));
	}
}
