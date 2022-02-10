namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		/// <summary>
		/// Executes one parser after another.
		/// </summary>
		public static IParser<TAfter> Then<TBefore, TAfter>(this IParser<TBefore> parser, Func<TBefore, IParser<TAfter>> convertValueToNextParser)
			=> Create(position => parser.TryParse(position).MapSuccess(result => convertValueToNextParser(result.Value).TryParse(result.NextPosition)));

		/// <summary>
		/// Converts any successfully parsed value.
		/// </summary>
		public static IParser<TAfter> Select<TBefore, TAfter>(this IParser<TBefore> parser, Func<TBefore, TAfter> convertValue)
			=> parser.Then(value => Success(convertValue(value)));

		/// <summary>
		/// Succeeds with the specified value if the parser is successful.
		/// </summary>
		public static IParser<TAfter> Success<TBefore, TAfter>(this IParser<TBefore> parser, TAfter value) => parser.Select(_ => value);

		/// <summary>
		/// Concatenates the two successfully parsed collections.
		/// </summary>
		public static IParser<IReadOnlyList<T>> Concat<T>(this IParser<IEnumerable<T>> firstParser, IParser<IEnumerable<T>> secondParser)
			=> firstParser.Then(firstValue => secondParser.Select(x => firstValue.Concat(x).ToList()));

		/// <summary>
		/// Appends a successfully parsed value to the end of a successfully parsed collection.
		/// </summary>
		public static IParser<IReadOnlyList<T>> Append<T>(this IParser<IEnumerable<T>> firstParser, IParser<T> secondParser)
			=> firstParser.Concat(secondParser.Once());

		/// <summary>
		/// Used to support LINQ query syntax.
		/// </summary>
		public static IParser<TAfter> SelectMany<TBefore, TDuring, TAfter>(this IParser<TBefore> parser, Func<TBefore, IParser<TDuring>> selector, Func<TBefore, TDuring, TAfter> projector)
			=> parser.Then(t => selector(t).Select(u => projector(t, u)));
	}
}
