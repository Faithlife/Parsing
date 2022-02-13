using System.Diagnostics.CodeAnalysis;

namespace Faithlife.Parsing;

[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1414:Tuple types in signatures should have element names", Justification = "No better names.")]
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
		return new ThenCreateParser<TBefore, TAfter>(parser, convertValueToNextParser);
	}

	private sealed class ThenCreateParser<TBefore, TAfter> : Parser<TAfter>
	{
		public ThenCreateParser(IParser<TBefore> parser, Func<TBefore, IParser<TAfter>> convertValueToNextParser)
		{
			m_parser = parser;
			m_convertValueToNextParser = convertValueToNextParser;
		}

		public override TAfter TryParse(ref TextPosition position, out bool success)
		{
			var value = m_parser.TryParse(ref position, out success);
			return success ? m_convertValueToNextParser(value).TryParse(ref position, out success) : default!;
		}

		private readonly IParser<TBefore> m_parser;
		private readonly Func<TBefore, IParser<TAfter>> m_convertValueToNextParser;
	}

	/// <summary>
	/// Executes one parser after another.
	/// </summary>
	public static IParser<(T1, T2)> Then<T1, T2>(this IParser<T1> parser, IParser<T2> nextParser)
	{
		if (parser is null)
			throw new ArgumentNullException(nameof(parser));
		if (nextParser is null)
			throw new ArgumentNullException(nameof(nextParser));
		return new ThenTupleParser<T1, T2>(parser, nextParser);
	}

	private sealed class ThenTupleParser<T1, T2> : Parser<(T1, T2)>
	{
		public ThenTupleParser(IParser<T1> parser, IParser<T2> nextParser)
		{
			m_parser = parser;
			m_nextParser = nextParser;
		}

		protected override (T1, T2) TryParse(ref TextPosition position, out bool success)
		{
			var value = m_parser.TryParse(ref position, out success);
			if (!success)
				return default!;

			var nextValue = m_nextParser.TryParse(ref position, out success);
			if (!success)
				return default!;

			return (value, nextValue);
		}

		private readonly IFastParser<T1> m_parser;
		private readonly IFastParser<T2> m_nextParser;
	}

	/// <summary>
	/// Executes one parser after another.
	/// </summary>
	public static IParser<TAfter> Then<T1, T2, TAfter>(this IParser<T1> parser, IParser<T2> nextParser, Func<T1, T2, TAfter> combineValues)
	{
		if (parser is null)
			throw new ArgumentNullException(nameof(parser));
		if (nextParser is null)
			throw new ArgumentNullException(nameof(nextParser));
		return new ThenParser<T1, T2, TAfter>(parser, nextParser, combineValues);
	}

	private sealed class ThenParser<T1, T2, TAfter> : Parser<TAfter>
	{
		public ThenParser(IParser<T1> parser, IParser<T2> nextParser, Func<T1, T2, TAfter> combineValues)
		{
			m_parser = parser;
			m_nextParser = nextParser;
			m_combineValues = combineValues;
		}

		public override TAfter TryParse(ref TextPosition position, out bool success)
		{
			var value = m_parser.TryParse(ref position, out success);
			if (!success)
				return default!;

			var nextValue = m_nextParser.TryParse(ref position, out success);
			if (!success)
				return default!;

			return m_combineValues(value, nextValue);
		}

		private readonly IParser<T1> m_parser;
		private readonly IParser<T2> m_nextParser;
		private readonly Func<T1, T2, TAfter> m_combineValues;
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
		return new SelectParser<TBefore, TAfter>(parser, convertValue);
	}

	private sealed class SelectParser<TBefore, TAfter> : Parser<TAfter>
	{
		public SelectParser(IParser<TBefore> parser, Func<TBefore, TAfter> convertValue) => (m_parser, m_convertValue) = (parser, convertValue);

		public override TAfter TryParse(ref TextPosition position, out bool success)
		{
			var value = m_parser.TryParse(ref position, out success);
			return success ? m_convertValue(value) : default!;
		}

		private readonly IParser<TBefore> m_parser;
		private readonly Func<TBefore, TAfter> m_convertValue;
	}

	/// <summary>
	/// Succeeds with the specified value if the parser is successful.
	/// </summary>
	public static IParser<TAfter> Success<TBefore, TAfter>(this IParser<TBefore> parser, TAfter value)
	{
		if (parser is null)
			throw new ArgumentNullException(nameof(parser));
		return new SuccessParser<TBefore, TAfter>(parser, value);
	}

	private sealed class SuccessParser<TBefore, TAfter> : Parser<TAfter>
	{
		public SuccessParser(IParser<TBefore> parser, TAfter value) => (m_parser, m_value) = (parser, value);

		public override TAfter TryParse(ref TextPosition position, out bool success)
		{
			m_parser.TryParse(ref position, out success);
			return success ? m_value : default!;
		}

		private readonly IParser<TBefore> m_parser;
		private readonly TAfter m_value;
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
