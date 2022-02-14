using System.Diagnostics.CodeAnalysis;

namespace Faithlife.Parsing;

[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1414:Tuple types in signatures should have element names", Justification = "No better names.")]
[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1128:Put constructor initializers on their own line", Justification = "More compact.")]
[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1502:Element should not be on a single line", Justification = "More compact.")]
public static partial class Parser
{
	/// <summary>
	/// Executes one parser after another.
	/// </summary>
	public static IParser<TAfter> Then<TBefore, TAfter>(this IParser<TBefore> parser, Func<TBefore, IParser<TAfter>> convertValueToNextParser) =>
		new ThenCreateParser<TBefore, TAfter>(parser, convertValueToNextParser);

	private sealed class ThenCreateParser<TBefore, TAfter> : Parser<TAfter>
	{
		public ThenCreateParser(IParser<TBefore> parser, Func<TBefore, IParser<TAfter>> convertValueToNextParser)
		{
			m_parser = parser ?? throw new ArgumentNullException(nameof(parser));
			m_convertValueToNextParser = convertValueToNextParser ?? throw new ArgumentNullException(nameof(convertValueToNextParser));
		}

		public override TAfter TryParse(bool skip, ref TextPosition position, out bool success)
		{
			var value = m_parser.TryParse(skip, ref position, out success);
			return success ? m_convertValueToNextParser(value).TryParse(skip, ref position, out success) : default!;
		}

		private readonly IParser<TBefore> m_parser;
		private readonly Func<TBefore, IParser<TAfter>> m_convertValueToNextParser;
	}

	/// <summary>
	/// Executes one parser after another.
	/// </summary>
	public static IParser<TAfter> Then<T1, T2, TAfter>(this IParser<T1> parser, IParser<T2> nextParser, Func<T1, T2, TAfter> combineValues) =>
		new FuncThenParser<T1, T2, TAfter>(parser, nextParser, combineValues);

	private abstract class ThenParser<T1, T2, TAfter> : Parser<TAfter>
	{
		protected ThenParser(IParser<T1> parser, IParser<T2> nextParser)
		{
			m_parser = parser ?? throw new ArgumentNullException(nameof(parser));
			m_nextParser = nextParser ?? throw new ArgumentNullException(nameof(nextParser));
		}

		public override TAfter TryParse(bool skip, ref TextPosition position, out bool success)
		{
			var value = m_parser.TryParse(skip, ref position, out success);
			if (!success)
				return default!;

			var nextValue = m_nextParser.TryParse(skip, ref position, out success);
			if (!success)
				return default!;

			return skip ? default! : CombineValues(value, nextValue);
		}

		protected abstract TAfter CombineValues(T1 value, T2 nextValue);

		private readonly IParser<T1> m_parser;
		private readonly IParser<T2> m_nextParser;
	}

	private sealed class FuncThenParser<T1, T2, TAfter> : ThenParser<T1, T2, TAfter>
	{
		public FuncThenParser(IParser<T1> parser, IParser<T2> nextParser, Func<T1, T2, TAfter> combineValues)
			: base(parser, nextParser)
		{
			m_combineValues = combineValues;
		}

		protected override TAfter CombineValues(T1 value, T2 nextValue) => m_combineValues(value, nextValue);

		private readonly Func<T1, T2, TAfter> m_combineValues;
	}

	/// <summary>
	/// Executes one parser after another.
	/// </summary>
	public static IParser<(T1, T2)> Then<T1, T2>(this IParser<T1> parser, IParser<T2> nextParser) =>
		new ThenTupleParser<T1, T2>(parser, nextParser);

	private sealed class ThenTupleParser<T1, T2> : ThenParser<T1, T2, (T1, T2)>
	{
		public ThenTupleParser(IParser<T1> parser, IParser<T2> nextParser) : base(parser, nextParser) { }
		protected override (T1, T2) CombineValues(T1 value, T2 nextValue) =>
			(value, nextValue);
	}

	/// <summary>
	/// Executes one parser after another.
	/// </summary>
	public static IParser<(T1, T2, T3)> Then<T1, T2, T3>(this IParser<(T1, T2)> parser, IParser<T3> nextParser) =>
		new ThenTupleParser<T1, T2, T3>(parser, nextParser);

	private sealed class ThenTupleParser<T1, T2, T3> : ThenParser<(T1, T2), T3, (T1, T2, T3)>
	{
		public ThenTupleParser(IParser<(T1, T2)> parser, IParser<T3> nextParser) : base(parser, nextParser) { }
		protected override (T1, T2, T3) CombineValues((T1, T2) value, T3 nextValue) =>
			(value.Item1, value.Item2, nextValue);
	}

	/// <summary>
	/// Executes one parser after another.
	/// </summary>
	public static IParser<(T1, T2, T3, T4)> Then<T1, T2, T3, T4>(this IParser<(T1, T2, T3)> parser, IParser<T4> nextParser) =>
		new ThenTupleParser<T1, T2, T3, T4>(parser, nextParser);

	private sealed class ThenTupleParser<T1, T2, T3, T4> : ThenParser<(T1, T2, T3), T4, (T1, T2, T3, T4)>
	{
		public ThenTupleParser(IParser<(T1, T2, T3)> parser, IParser<T4> nextParser) : base(parser, nextParser) { }
		protected override (T1, T2, T3, T4) CombineValues((T1, T2, T3) value, T4 nextValue) =>
			(value.Item1, value.Item2, value.Item3, nextValue);
	}

	/// <summary>
	/// Executes one parser after another.
	/// </summary>
	public static IParser<(T1, T2, T3, T4, T5)> Then<T1, T2, T3, T4, T5>(this IParser<(T1, T2, T3, T4)> parser, IParser<T5> nextParser) =>
		new ThenTupleParser<T1, T2, T3, T4, T5>(parser, nextParser);

	private sealed class ThenTupleParser<T1, T2, T3, T4, T5> : ThenParser<(T1, T2, T3, T4), T5, (T1, T2, T3, T4, T5)>
	{
		public ThenTupleParser(IParser<(T1, T2, T3, T4)> parser, IParser<T5> nextParser) : base(parser, nextParser) { }
		protected override (T1, T2, T3, T4, T5) CombineValues((T1, T2, T3, T4) value, T5 nextValue) =>
			(value.Item1, value.Item2, value.Item3, value.Item4, nextValue);
	}

	/// <summary>
	/// Executes one parser after another.
	/// </summary>
	public static IParser<(T1, T2, T3, T4, T5, T6)> Then<T1, T2, T3, T4, T5, T6>(this IParser<(T1, T2, T3, T4, T5)> parser, IParser<T6> nextParser) =>
		new ThenTupleParser<T1, T2, T3, T4, T5, T6>(parser, nextParser);

	private sealed class ThenTupleParser<T1, T2, T3, T4, T5, T6> : ThenParser<(T1, T2, T3, T4, T5), T6, (T1, T2, T3, T4, T5, T6)>
	{
		public ThenTupleParser(IParser<(T1, T2, T3, T4, T5)> parser, IParser<T6> nextParser) : base(parser, nextParser) { }
		protected override (T1, T2, T3, T4, T5, T6) CombineValues((T1, T2, T3, T4, T5) value, T6 nextValue) =>
			(value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, nextValue);
	}

	/// <summary>
	/// Executes one parser after another.
	/// </summary>
	public static IParser<(T1, T2, T3, T4, T5, T6, T7)> Then<T1, T2, T3, T4, T5, T6, T7>(this IParser<(T1, T2, T3, T4, T5, T6)> parser, IParser<T7> nextParser) =>
		new ThenTupleParser<T1, T2, T3, T4, T5, T6, T7>(parser, nextParser);

	private sealed class ThenTupleParser<T1, T2, T3, T4, T5, T6, T7> : ThenParser<(T1, T2, T3, T4, T5, T6), T7, (T1, T2, T3, T4, T5, T6, T7)>
	{
		public ThenTupleParser(IParser<(T1, T2, T3, T4, T5, T6)> parser, IParser<T7> nextParser) : base(parser, nextParser) { }
		protected override (T1, T2, T3, T4, T5, T6, T7) CombineValues((T1, T2, T3, T4, T5, T6) value, T7 nextValue) =>
			(value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6, nextValue);
	}

	/// <summary>
	/// Executes one parser after another.
	/// </summary>
	public static IParser<(T1, T2, T3, T4, T5, T6, T7, T8)> Then<T1, T2, T3, T4, T5, T6, T7, T8>(this IParser<(T1, T2, T3, T4, T5, T6, T7)> parser, IParser<T8> nextParser) =>
		new ThenTupleParser<T1, T2, T3, T4, T5, T6, T7, T8>(parser, nextParser);

	private sealed class ThenTupleParser<T1, T2, T3, T4, T5, T6, T7, T8> : ThenParser<(T1, T2, T3, T4, T5, T6, T7), T8, (T1, T2, T3, T4, T5, T6, T7, T8)>
	{
		public ThenTupleParser(IParser<(T1, T2, T3, T4, T5, T6, T7)> parser, IParser<T8> nextParser) : base(parser, nextParser) { }
		protected override (T1, T2, T3, T4, T5, T6, T7, T8) CombineValues((T1, T2, T3, T4, T5, T6, T7) value, T8 nextValue) =>
			(value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6, value.Item7, nextValue);
	}

	/// <summary>
	/// Executes one parser after another, ignoring the output of the second parser.
	/// </summary>
	public static IParser<T1> ThenSkip<T1, T2>(this IParser<T1> parser, IParser<T2> nextParser) =>
		new ThenSkipTupleParser<T1, T2>(parser, nextParser);

	private sealed class ThenSkipTupleParser<T1, T2> : Parser<T1>
	{
		public ThenSkipTupleParser(IParser<T1> parser, IParser<T2> nextParser)
		{
			m_parser = parser ?? throw new ArgumentNullException(nameof(parser));
			m_nextParser = nextParser ?? throw new ArgumentNullException(nameof(nextParser));
		}

		public override T1 TryParse(bool skip, ref TextPosition position, out bool success)
		{
			var value = m_parser.TryParse(skip, ref position, out success);
			if (!success)
				return default!;

			m_nextParser.TryParse(skip: true, ref position, out success);
			if (!success)
				return default!;

			return value;
		}

		private readonly IParser<T1> m_parser;
		private readonly IParser<T2> m_nextParser;
	}

	/// <summary>
	/// Executes one parser after another, ignoring the output of the first parser.
	/// </summary>
	public static IParser<T2> SkipThen<T1, T2>(this IParser<T1> parser, IParser<T2> nextParser) =>
		new SkipThenTupleParser<T1, T2>(parser, nextParser);

	private sealed class SkipThenTupleParser<T1, T2> : Parser<T2>
	{
		public SkipThenTupleParser(IParser<T1> parser, IParser<T2> nextParser)
		{
			m_parser = parser ?? throw new ArgumentNullException(nameof(parser));
			m_nextParser = nextParser ?? throw new ArgumentNullException(nameof(nextParser));
		}

		public override T2 TryParse(bool skip, ref TextPosition position, out bool success)
		{
			m_parser.TryParse(skip: true, ref position, out success);
			if (!success)
				return default!;

			var nextValue = m_nextParser.TryParse(skip, ref position, out success);
			if (!success)
				return default!;

			return nextValue;
		}

		private readonly IParser<T1> m_parser;
		private readonly IParser<T2> m_nextParser;
	}

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

		public override TAfter TryParse(bool skip, ref TextPosition position, out bool success)
		{
			var value = m_parser.TryParse(skip, ref position, out success);
			return success && !skip ? m_convertValue(value) : default!;
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

		public override TAfter TryParse(bool skip, ref TextPosition position, out bool success)
		{
			m_parser.TryParse(skip: true, ref position, out success);
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
